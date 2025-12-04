using EmpDir.Core.Models;

public interface IDirectoryService
{

    /// Service interface for directory operations
    /// Implemented by:
    /// - DirectoryService (EmpDir.Core) - uses AppDbContext for Admin and API
    /// - CachedDirectoryService (EmpDir.Desktop) - uses LocalCacheContext for Desktop
   
    Task<List<Department>> GetDepartmentsAsync();
    Task<Department?> GetDepartmentByIdAsync(int id);
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<List<Location>> GetLocationsByTypeAsync(string loctypeName);
    Task<Location?> GetLocationWithDepartmentsAsync(int locationId);
    Task<Department?> GetDepartmentWithEmployeesAsync(int departmentId);
    Task<Location?> GetLocationByIdAsync(string loctypeName, int id);
    Task<Location?> GetLocationByIdAsync(int id);
    Task<List<Loctype>> GetLoctypesAsync();
    Task<List<Location>> GetLocationsAsync();
    Task<List<Employee>> GetEmployeesAsync();
    Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task<List<Employee>> GetEmployeesByLocationAsync(int locationId);
    Task<List<Employee>> GetEmployeesByLocationAndDepartmentAsync(int locationId, int departmentId);



}

