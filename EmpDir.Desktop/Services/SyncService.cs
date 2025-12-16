using EmpDir.Core.Extensions;
using EmpDir.Core.Services;
using Microsoft.Extensions.Logging;

namespace EmpDir.Desktop.Services;

// Service for syncing data from API to local cache
public class SyncService : ISyncService
{
    private readonly IApiService _apiService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<SyncService> _logger;

    private DateTime? _lastSyncTime;
    private bool _isSyncing;

    public DateTime? LastSyncTime => _lastSyncTime;
    public bool IsSyncing => _isSyncing;

    public SyncService(
        IApiService apiService,
        ICacheService cacheService,
        ILogger<SyncService> logger)
    {
        _apiService = apiService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<SyncResult> SyncOnLaunchAsync()
    {
        if (_isSyncing)
        {
            _logger.LogWarning("Sync already in progress");
            return new SyncResult
            {
                Success = false,
                Message = "Sync already in progress",
                SyncTime = DateTime.Now
            };
        }

        _isSyncing = true;

        try
        {
            _logger.LogInformation("Starting directory sync...");

            // Check API availability
            var apiAvailable = await _apiService.HealthCheckAsync();

            if (!apiAvailable)
            {
                _logger.LogWarning("API is not available. Using cached data.");

                var lastSync = await _cacheService.GetLastSyncTimeAsync();

                return new SyncResult
                {
                    Success = true,
                    Message = lastSync.HasValue
                        ? $"Offline - using cache from {lastSync.Value:g}"
                        : "Offline - no cached data available",
                    ApiWasAvailable = false,
                    SyncTime = DateTime.Now
                };
            }

            // Fetch data from API
            var syncData = await _apiService.GetFullDirectoryAsync();

            if (syncData == null)
            {
                _logger.LogError("Failed to retrieve data from API");
                return new SyncResult
                {
                    Success = false,
                    Message = "Failed to retrieve data from API",
                    ApiWasAvailable = true,
                    SyncTime = DateTime.Now
                };
            }

            // Convert DTOs to models
            var employees = syncData.Employees.Select(dto => dto.ToModel()).ToList();
            var departments = syncData.Departments.Select(dto => dto.ToModel()).ToList();
            var locations = syncData.Locations.Select(dto => dto.ToModel()).ToList();
            var locationTypes = syncData.LocationTypes.Select(dto => dto.ToModel()).ToList();

            // Break navigation property links to avoid EF tracking issues
            foreach (var loc in locations)
            {
                loc.LocationType = null;
            }

            foreach (var dept in departments)
            {
                dept.DeptLocation = null;
            }

            foreach (var emp in employees)
            {
                emp.EmpLocation = null;
                emp.EmpDepartment = null;
            }

            // Clear existing cache before inserting fresh data
            await _cacheService.ClearAllDataAsync();

            // Save to cache in dependency order
            await _cacheService.SaveLocationTypesAsync(locationTypes);
            await _cacheService.SaveLocationsAsync(locations);
            await _cacheService.SaveDepartmentsAsync(departments);
            await _cacheService.SaveEmployeesAsync(employees);

            // Update sync metadata
            var syncTime = DateTime.Now;
            await _cacheService.SetLastSyncTimeAsync(syncTime);
            _lastSyncTime = syncTime;

            _logger.LogInformation("Sync completed successfully at {SyncTime}", syncTime);

            return new SyncResult
            {
                Success = true,
                Message = "Sync completed successfully",
                EmployeesUpdated = employees.Count,
                DepartmentsUpdated = departments.Count,
                LocationsUpdated = locations.Count,
                LocationTypesUpdated = locationTypes.Count,
                ApiWasAvailable = true,
                SyncTime = syncTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sync operation");

            return new SyncResult
            {
                Success = false,
                Message = $"Sync failed: {ex.Message}",
                SyncTime = DateTime.Now
            };
        }
        finally
        {
            _isSyncing = false;
        }
    }

    public async Task<bool> IsApiAvailableAsync()
    {
        return await _apiService.HealthCheckAsync();
    }
}