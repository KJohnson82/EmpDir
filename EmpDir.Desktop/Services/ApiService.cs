using EmpDir.Core.DTOs;
using EmpDir.Core.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace EmpDir.Desktop.Services;

/// <summary>
/// Service for communicating with EmpDir.Api
/// </summary>
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
            _logger.LogError(ex, "Network error when calling API");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "API request timed out");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize API response");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when calling API");
            return null;
        }
    }

    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("EmpDirApi");
            var response = await client.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public string GetApiUrl()
    {
        var client = _httpClientFactory.CreateClient("EmpDirApi");
        return client.BaseAddress?.ToString() ?? "Unknown";
    }
}