using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using EmpDir.Core.Models;
using EmpDir.Core.DTOs;
using EmpDir.Core.Extensions;

namespace EmpDir.Core.Services;

/// <summary>
/// Implementation of IDirectoryService using Entity Framework Core
/// Used by: EmpDir.Api and EmpDir.Admin (direct database access)
/// </summary>
public class DirectoryService : IDirectoryService
{
    private readonly AppDbContext _context;

    public DirectoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LocationDto?> GetLocationWithDepartmentsAsync(int locationId)
    {
        var location = await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .Include(l => l.Departments.Where(d => d.Active == true))
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == locationId);

        return location?.ToDto();
    }

    public async Task<DepartmentDto?> GetDepartmentWithEmployeesAsync(int departmentId)
    {
        var department = await _context.Departments
            .Where(d => d.Active == true)
            .Include(d => d.DeptLocation)
            .Include(d => d.Employees.Where(e => e.Active == true))
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == departmentId);

        return department?.ToDto();
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int employeeId)
    {
        var employee = await _context.Employees
            .Where(e => e.Active == true)
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        return employee?.ToDto();
    }

    public async Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        var employees = await _context.Employees
            .Where(e => e.Active == true && e.Department == departmentId)
            .Include(e => e.EmpLocation)
            .Include(e => e.EmpDepartment)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<List<DepartmentDto>> GetDepartmentsAsync()
    {
        var departments = await _context.Departments
            .Where(d => d.Active == true)
            .Include(d => d.DeptLocation)
            .AsNoTracking()
            .ToListAsync();

        return departments.Select(d => d.ToDto()).ToList();
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        var department = await _context.Departments
            .Where(d => d.Active == true)
            .Include(d => d.DeptLocation)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        return department?.ToDto();
    }

    public async Task<List<LocationDto>> GetLocationsByTypeAsync(string loctypeName)
    {
        var locations = await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .Where(l => l.LocationType!.LoctypeName.ToLower() == loctypeName.ToLower())
            .AsNoTracking()
            .ToListAsync();

        return locations.Select(l => l.ToDto()).ToList();
    }

    public async Task<LocationDto?> GetLocationByIdAsync(string loctypeName, int id)
    {
        var location = await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .Where(l => l.LocationType!.LoctypeName.ToLower() == loctypeName.ToLower() && l.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return location?.ToDto();
    }

    public async Task<List<LoctypeDto>> GetLoctypesAsync()
    {
        var loctypes = await _context.Loctypes
            .AsNoTracking()
            .ToListAsync();

        return loctypes.Select(lt => lt.ToDto()).ToList();
    }

    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        var locations = await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .AsNoTracking()
            .ToListAsync();

        return locations.Select(l => l.ToDto()).ToList();
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _context.Locations
            .Where(l => l.Active == true)
            .Include(l => l.LocationType)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        return location?.ToDto();
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        var employees = await _context.Employees
            .Where(e => e.Active == true)
            .Include(e => e.EmpDepartment)
            .Include(e => e.EmpLocation)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<List<EmployeeDto>> GetEmployeesByLocationAsync(int locationId)
    {
        var employees = await _context.Employees
            .Where(e => e.Active == true && e.Location == locationId)
            .Include(e => e.EmpDepartment)
            .Include(e => e.EmpLocation)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }

    public async Task<List<EmployeeDto>> GetEmployeesByLocationAndDepartmentAsync(int locationId, int departmentId)
    {
        var employees = await _context.Employees
            .Where(e => e.Active == true && e.Location == locationId && e.Department == departmentId)
            .Include(e => e.EmpDepartment)
            .Include(e => e.EmpLocation)
            .AsNoTracking()
            .ToListAsync();

        return employees.Select(e => e.ToDto()).ToList();
    }
}