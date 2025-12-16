using EmpDir.Core.Models;
using EmpDir.Core.Services;
using EmpDir.Desktop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Location = EmpDir.Core.Models.Location;

namespace EmpDir.Desktop.Services;

// Desktop implementation of ISearchService that searches the local SQLite cache
public class CachedSearchService : ISearchService
{
    private readonly LocalCacheContext _context;
    private readonly ILogger<CachedSearchService> _logger;

    public CachedSearchService(
        LocalCacheContext context,
        ILogger<CachedSearchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Performs comprehensive search across employees, departments, and locations
    public async Task<SearchResults> SearchAllAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new SearchResults();

        _logger.LogDebug("Starting cached search for term: '{SearchTerm}'", searchTerm);

        try
        {
            var employees = await SearchEmployeesAsync(searchTerm);
            var departments = await SearchDepartmentsAsync(searchTerm);
            var locations = await SearchLocationsAsync(searchTerm);

            _logger.LogDebug("Found {EmployeeCount} employees, {DepartmentCount} departments, {LocationCount} locations",
                employees.Count, departments.Count, locations.Count);

            return new SearchResults
            {
                Employees = employees,
                Departments = departments,
                Locations = locations,
                SearchTerm = searchTerm
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search error for term: '{SearchTerm}'", searchTerm);
            return new SearchResults { SearchTerm = searchTerm };
        }
    }

    private async Task<List<Employee>> SearchEmployeesAsync(string searchTerm)
    {
        try
        {
            var allEmployees = await _context.Employees
                .Include(e => e.EmpLocation)
                .Include(e => e.EmpDepartment)
                .Where(e => e.Active == true)
                .ToListAsync();

            return allEmployees.Where(e =>
                (e.FirstName != null && e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (e.LastName != null && e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (e.JobTitle != null && e.JobTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (e.PhoneNumber != null && e.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (e.CellNumber != null && e.CellNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (e.Email != null && e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (e.Extension != null && e.Extension.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).Take(20).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Employee search error");
            return new List<Employee>();
        }
    }

    private async Task<List<Department>> SearchDepartmentsAsync(string searchTerm)
    {
        try
        {
            var allDepartments = await _context.Departments
                .Include(d => d.DeptLocation)
                .Where(d => d.Active == true)
                .ToListAsync();

            return allDepartments.Where(d =>
                (d.DeptName != null && d.DeptName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (d.DeptManager != null && d.DeptManager.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (d.DeptPhone != null && d.DeptPhone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (d.DeptEmail != null && d.DeptEmail.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).Take(20).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Department search error");
            return new List<Department>();
        }
    }

    private async Task<List<Location>> SearchLocationsAsync(string searchTerm)
    {
        try
        {
            var allLocations = await _context.Locations
                .Include(l => l.LocationType)
                .Where(l => l.Active == true)
                .ToListAsync();

            return allLocations.Where(l =>
                (l.LocName != null && l.LocName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.Address != null && l.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.City != null && l.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.State != null && l.State.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.Zipcode != null && l.Zipcode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.PhoneNumber != null && l.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.AreaManager != null && l.AreaManager.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.StoreManager != null && l.StoreManager.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (l.LocNum.HasValue && l.LocNum.ToString()!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).Take(20).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Location search error");
            return new List<Location>();
        }
    }
}