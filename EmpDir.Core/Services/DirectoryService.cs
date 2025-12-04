
using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using EmpDir.Core.Models;
using EmpDir.Core.Services;

/// Implementation of IDirectoryService using Entity Framework Core
/// Used by: EmpDir.Api and EmpDir.Admin (direct database access)
/// NOT used by: EmpDir.Desktop (uses CachedDirectoryService instead)

namespace EmpDir.Core.Services;

public class DirectoryService : IDirectoryService
{
    private readonly AppDbContext _context;
    public DirectoryService(AppDbContext context)
    {
        _context = context;
    }

    
    /// Gets location with its active departments only
    
    public async Task<Location?> GetLocationWithDepartmentsAsync(int locationId) =>
        await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.Departments.Where(d => d.Active == true)) // Filter departments by Active
            .FirstOrDefaultAsync(l => l.Id == locationId);

    
    /// Gets department with its active employees only
    
    public async Task<Department?> GetDepartmentWithEmployeesAsync(int departmentId) =>
        await _context.Departments
            .Where(d => d.Active == true)
            .Include(d => d.Employees.Where(e => e.Active == true)) // Filter employees by Active
            .FirstOrDefaultAsync(d => d.Id == departmentId);

    
    /// Gets employee by ID (only if active)
    
    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId) =>
        await _context.Employees
            .Where(e => e.Active == true) // Add Active filter
            .FirstOrDefaultAsync(e => e.Id == employeeId);

    
    /// Gets active employees by department
    
    public async Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId) =>
        await _context.Employees
            .Where(e => e.Active == true) // Add Active filter
            .Where(e => e.Department == departmentId)
            .ToListAsync();

    
    /// Gets all active departments
    
    public async Task<List<Department>> GetDepartmentsAsync() =>
        await _context.Departments
            .Where(d => d.Active == true)
            .Include(d => d.DeptLocation)
            .ToListAsync();

    
    /// Gets active department by ID
    
    public async Task<Department?> GetDepartmentByIdAsync(int id) =>
        await _context.Departments
            .Where(d => d.Active == true)
            .Include(d => d.DeptLocation)
            .FirstOrDefaultAsync(d => d.Id == id);

    
    /// Gets active locations by type
    
    public async Task<List<Location>> GetLocationsByTypeAsync(string loctypeName)
    {
        return await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .Where(l => l.LocationType!.LoctypeName.ToLower() == loctypeName.ToLower())
            .ToListAsync();
    }

    
    /// Gets active location by type and ID
    
    public async Task<Location?> GetLocationByIdAsync(string loctypeName, int id)
    {
        return await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .Where(l => l.LocationType!.LoctypeName.ToLower() == loctypeName.ToLower() && l.Id == id)
            .FirstOrDefaultAsync();
    }

    
    /// Gets all location types (no Active filter needed)
    
    public async Task<List<Loctype>> GetLoctypesAsync()
    {
        return await _context.Loctypes.ToListAsync();
    }

    
    /// Gets all active locations
    
    public async Task<List<Location>> GetLocationsAsync()
    {
        return await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .ToListAsync();
    }

    
    /// Gets active location by ID
    
    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        return await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    
    /// Gets all active employees
    
    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await _context.Employees
            .Where(e => e.Active == true)
            .Include(e => e.EmpDepartment)
            .Include(e => e.EmpLocation)
            .ToListAsync();
    }

    
    /// Gets active employees by location
    
    public async Task<List<Employee>> GetEmployeesByLocationAsync(int locationId)
    {
        return await _context.Employees
            .Where(e => e.Active == true)
            .Where(e => e.Location == locationId)
            .ToListAsync();
    }

    
    /// Gets active employees by location and department
    
    public async Task<List<Employee>> GetEmployeesByLocationAndDepartmentAsync(int locationId, int departmentId)
    {
        return await _context.Employees
            .Where(e => e.Active == true)
            .Where(e => e.Location == locationId && e.Department == departmentId)
            .ToListAsync();
    }
}