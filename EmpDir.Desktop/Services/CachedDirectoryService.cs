using EmpDir.Core.Models;
using EmpDir.Desktop.Data;
using Microsoft.EntityFrameworkCore;
using Location = EmpDir.Core.Models.Location;

namespace EmpDir.Desktop.Services;

/// <summary>
/// Implementation of IDirectoryService that reads from local cache
/// Used by EmpDir.Desktop UI components
/// </summary>
public class CachedDirectoryService : IDirectoryService
{
    private readonly LocalCacheContext _context;

    public CachedDirectoryService(LocalCacheContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees)
            .Where(d => (bool)d.Active)
            .OrderBy(d => d.DeptName)
            .ToListAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Location>> GetLocationsByTypeAsync(string loctypeName)
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .Where(l => l.LocationType != null && l.LocationType.LoctypeName == loctypeName && l.Active == true)
            .OrderBy(l => l.LocName)
            .ToListAsync();
    }

    public async Task<Location?> GetLocationWithDepartmentsAsync(int locationId)
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .FirstOrDefaultAsync(l => l.Id == locationId);
    }

    public async Task<Department?> GetDepartmentWithEmployeesAsync(int departmentId)
    {
        return await _context.Departments
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == departmentId);
    }

    public async Task<Location?> GetLocationByIdAsync(string loctypeName, int id)
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .FirstOrDefaultAsync(l => l.Id == id && l.LocationType != null && l.LocationType.LoctypeName == loctypeName);
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Loctype>> GetLoctypesAsync()
    {
        return await _context.Loctypes
            .Include(lt => lt.Locations)
            .OrderBy(lt => lt.LoctypeName)
            .ToListAsync();
    }

    public async Task<List<Location>> GetLocationsAsync()
    {
        return await _context.Locations
            .Include(l => l.LocationType)
            .Include(l => l.Departments)
            .Include(l => l.Employees)
            .Where(l => (bool)l.Active)
            .OrderBy(l => l.LocName)
            .ToListAsync();
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => (bool)e.Active)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    public async Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => e.Department == departmentId && e.Active == true)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    public async Task<List<Employee>> GetEmployeesByLocationAsync(int locationId)
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => e.Location == locationId && e.Active == true)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    public async Task<List<Employee>> GetEmployeesByLocationAndDepartmentAsync(int locationId, int departmentId)
    {
        return await _context.Employees
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .Where(e => e.Location == locationId && e.Department == departmentId && e.Active == true)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }
}