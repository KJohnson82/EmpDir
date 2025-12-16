using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.DTOs
{

    /// Data Transfer Object for Department - used for API communication
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string DeptName { get; set; } = string.Empty;
        public int Location { get; set; }
        public string? DeptManager { get; set; }
        public string? DeptPhone { get; set; }
        public string? DeptEmail { get; set; }
        public string? DeptFax { get; set; }
        public DateTime? RecordAdd { get; set; }
        public bool Active { get; set; }

        // Flattened related data
        public string? LocationName { get; set; }

        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }

}
