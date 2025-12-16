using EmpDir.Core.DTOs;
using EmpDir.Core.Models;
namespace EmpDir.Core.Services;


public interface IDirectoryService
{
    Task<List<DepartmentDto>> GetDepartmentsAsync();
    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<List<LocationDto>> GetLocationsByTypeAsync(string loctypeName);
    Task<LocationDto?> GetLocationWithDepartmentsAsync(int locationId);
    Task<DepartmentDto?> GetDepartmentWithEmployeesAsync(int departmentId);
    Task<LocationDto?> GetLocationByIdAsync(string loctypeName, int id);
    Task<LocationDto?> GetLocationByIdAsync(int id);
    Task<List<LoctypeDto>> GetLoctypesAsync();
    Task<List<LocationDto>> GetLocationsAsync();
    Task<List<EmployeeDto>> GetEmployeesAsync();
    Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<List<EmployeeDto>> GetEmployeesByLocationAsync(int locationId);
    Task<List<EmployeeDto>> GetEmployeesByLocationAndDepartmentAsync(int locationId, int departmentId);
}