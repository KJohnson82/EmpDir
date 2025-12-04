using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.DTOs
{
    /// <summary>
    /// Container for full directory sync response from API
    /// </summary>
    public class DirectorySyncDto
    {
        public List<EmployeeDto> Employees { get; set; } = new();
        public List<DepartmentDto> Departments { get; set; } = new();
        public List<LocationDto> Locations { get; set; } = new();
        public List<LoctypeDto> LocationTypes { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

}
