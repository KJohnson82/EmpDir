using EmpDir.Core.DTOs;
using EmpDir.Core.Extensions;
using EmpDir.Core.Services;
using EmpDir.Desktop.Data;
using Microsoft.EntityFrameworkCore;

namespace EmpDir.Desktop.Services;

// IDirectoryService implementation that reads from local cache for EmpDir.Desktop
public class CachedDirectoryService : IDirectoryService
{
    private readonly LocalCacheContext _context;

    public CachedDirectoryService(LocalCacheContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentDto>> GetDepartmentsAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees)
            .Where(d => (bool)d.Active)
            .OrderBy(d => d.DeptName)
            .AsNoTracking()
            .ToListAsync();

        return departments.Select(d => d.ToDto()).ToList();
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        var department = await _context.Departments
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        return department?.ToDto();
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return employee?.ToDto();
    }

    public async Task<List<LocationDto>> GetLocationsByTypeAsync(string loctypeName)
    {
        var locations = await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .Where(l => l.LocationType != null
                && l.LocationType.LoctypeName.ToLower() == loctypeName.ToLower()
                && l.Active == true)
            .OrderBy(l => l.LocName)
            .AsNoTracking()
            .ToListAsync();

        return locations.Select(l => l.ToDto(includeRelated: true)).ToList();
    }

    public async Task<LocationDto?> GetLocationWithDepartmentsAsync(int locationId)
    {
        var location = await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == locationId);

        return location?.ToDto();
    }

    public async Task<DepartmentDto?> GetDepartmentWithEmployeesAsync(int departmentId)
    {
        var department = await _context.Departments
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == departmentId);

        return department?.ToDto();
    }

    public async Task<LocationDto?> GetLocationByIdAsync(string loctypeName, int id)
    {
        var location = await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id
                && l.LocationType != null
                && l.LocationType.LoctypeName.ToLower() == loctypeName.ToLower());

        return location?.ToDto(includeRelated: true);
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        return location?.ToDto();
    }

    public async Task<List<LoctypeDto>> GetLoctypesAsync()
    {
        var loctypes = await _context.Loctypes
            .Include(lt => lt.Locations)
            .OrderBy(lt => lt.LoctypeName)
            .AsNoTracking()
            .ToListAsync();

        return loctypes.Select(lt => lt.ToDto()).ToList();
    }

    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        var locations = await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .Where(l => (bool)l.Active)
            .OrderBy(l => l.LocName)
            .AsNoTracking()
            .ToListAsync();

        return locations.Select(l => l.ToDto()).ToList();
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => (bool)e.Active)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        var employees = await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => e.Department == departmentId && e.Active == true)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<List<EmployeeDto>> GetEmployeesByLocationAsync(int locationId)
    {
        var employees = await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => e.Location == locationId && e.Active == true)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<List<EmployeeDto>> GetEmployeesByLocationAndDepartmentAsync(int locationId, int departmentId)
    {
        var employees = await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => e.Location == locationId && e.Department == departmentId && e.Active == true)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }
}