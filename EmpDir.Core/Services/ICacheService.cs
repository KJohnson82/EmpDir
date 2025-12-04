using EmpDir.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.Services
{
    /// <summary>
    /// Service for managing local cache database
    /// Implemented in EmpDir.Desktop
    /// </summary>
    public interface ICacheService
    {
        // Write operations
        Task SaveEmployeesAsync(List<Employee> employees);
        Task SaveDepartmentsAsync(List<Department> departments);
        Task SaveLocationsAsync(List<Location> locations);
        Task SaveLocationTypesAsync(List<Loctype> locationTypes);
        Task ClearAllDataAsync();

        // Read operations
        Task<List<Employee>> GetEmployeesAsync();
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<Location>> GetLocationsAsync();
        Task<List<Loctype>> GetLocationTypesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<Location?> GetLocationByIdAsync(int id);

        // Metadata
        Task<DateTime?> GetLastSyncTimeAsync();
        Task SetLastSyncTimeAsync(DateTime syncTime);
        Task<int> GetCachedRecordCountAsync();
    }

}
