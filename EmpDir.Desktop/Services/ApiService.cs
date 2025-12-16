using EmpDir.Core.DTOs;
using EmpDir.Core.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;

namespace EmpDir.Desktop.Services;

// Service for communicating with EmpDir.Api
public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiService> _logger;

    public ApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<ApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<DirectorySyncDto?> GetFullDirectoryAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("EmpDirApi");

            _logger.LogInformation("Requesting full directory sync from API: {BaseUrl}", client.BaseAddress);

            var response = await client.GetAsync("/api/directory/sync");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<DirectorySyncDto>();

                if (data != null)
                {
                    _logger.LogInformation(
                        "Successfully retrieved directory: {EmployeeCount} employees, {DeptCount} departments, {LocCount} locations",
                        data.Employees.Count,
                        data.Departments.Count,
                        data.Locations.Count);
                }

                return data;
            }
            else
            {
                _logger.LogWarning("API returned status code: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error when calling API - server may be unavailable");
            return null;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning("API request timed out");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "API request was cancelled");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize API response");
            return null;
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "I/O error when calling API - connection may have been closed");
            return null;
        }
        catch (SocketException ex)
        {
            _logger.LogWarning(ex, "Socket error when calling API - network issue");
            return null;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "API operation was cancelled");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when calling API: {ErrorType}", ex.GetType().Name);
            return null;
        }
    }

    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("EmpDirApi");

            // Use shorter timeout for health checks
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await client.GetAsync("/health", cts.Token);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (SocketException)
        {
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Health check unexpected error: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    public string GetApiUrl()
    {
        var client = _httpClientFactory.CreateClient("EmpDirApi");
        return client.BaseAddress?.ToString() ?? "Unknown";
    }
}