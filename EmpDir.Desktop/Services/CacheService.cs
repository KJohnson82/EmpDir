using EmpDir.Core.Models;
using EmpDir.Core.Services;
using EmpDir.Desktop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Location = EmpDir.Core.Models.Location;

namespace EmpDir.Desktop.Services;

// Service for managing local cache database
public class CacheService : ICacheService
{
    private readonly LocalCacheContext _context;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        LocalCacheContext context,
        ILogger<CacheService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Write operations
    public async Task SaveEmployeesAsync(List<Employee> employees)
    {
        try
        {
            _context.Employees.RemoveRange(_context.Employees);
            await _context.Employees.AddRangeAsync(employees);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved {Count} employees to cache", employees.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save employees to cache");
            throw;
        }
    }

    public async Task SaveDepartmentsAsync(List<Department> departments)
    {
        try
        {
            _context.Departments.RemoveRange(_context.Departments);
            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved {Count} departments to cache", departments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save departments to cache");
            throw;
        }
    }

    public async Task SaveLocationsAsync(List<Location> locations)
    {
        try
        {
            _context.Locations.RemoveRange(_context.Locations);
            await _context.Locations.AddRangeAsync(locations);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved {Count} locations to cache", locations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save locations to cache");
            throw;
        }
    }

    public async Task SaveLocationTypesAsync(List<Loctype> locationTypes)
    {
        try
        {
            _context.Loctypes.RemoveRange(_context.Loctypes);
            await _context.Loctypes.AddRangeAsync(locationTypes);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved {Count} location types to cache", locationTypes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save location types to cache");
            throw;
        }
    }

    public async Task ClearAllDataAsync()
    {
        try
        {
            _context.Employees.RemoveRange(_context.Employees);
            _context.Departments.RemoveRange(_context.Departments);
            _context.Locations.RemoveRange(_context.Locations);
            _context.Loctypes.RemoveRange(_context.Loctypes);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleared all data from cache");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cache");
            throw;
        }
    }

    // Read operations
    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .ToListAsync();
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .Include(d => d.DeptLocation)
            .ToListAsync();
    }

    public async Task<List<Location>> GetLocationsAsync()
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .ToListAsync();
    }

    public async Task<List<Loctype>> GetLocationTypesAsync()
    {
        return await _context.Loctypes.ToListAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments
            .Include(d => d.DeptLocation)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    // Metadata operations
    public async Task<DateTime?> GetLastSyncTimeAsync()
    {
        var metadata = await _context.SyncMetadata
            .OrderByDescending(sm => sm.LastSyncTime)
            .FirstOrDefaultAsync();

        return metadata?.LastSyncTime;
    }

    public async Task SetLastSyncTimeAsync(DateTime syncTime)
    {
        var metadata = new SyncMetadata
        {
            LastSyncTime = syncTime,
            EmployeeCount = await _context.Employees.CountAsync(),
            DepartmentCount = await _context.Departments.CountAsync(),
            LocationCount = await _context.Locations.CountAsync(),
            LoctypeCount = await _context.Loctypes.CountAsync(),
            LastSyncSuccessful = true,
            LastSyncMessage = "Sync completed successfully"
        };

        await _context.SyncMetadata.AddAsync(metadata);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCachedRecordCountAsync()
    {
        var employeeCount = await _context.Employees.CountAsync();
        var deptCount = await _context.Departments.CountAsync();
        var locCount = await _context.Locations.CountAsync();

        return employeeCount + deptCount + locCount;
    }
}