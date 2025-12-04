using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.DTOs
{
    /// <summary>
    /// Data Transfer Object for Employee - used for API communication
    /// </summary>
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public bool IsManager { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? CellNumber { get; set; }
        public string? Extension { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? NetworkId { get; set; }
        public string? EmpAvatar { get; set; }
        public int Location { get; set; }
        public int Department { get; set; }
        public DateTime? RecordAdd { get; set; }
        public bool Active { get; set; }

        // Flattened related data for display purposes
        public string? LocationName { get; set; }
        public string? DepartmentName { get; set; }
    }

}
