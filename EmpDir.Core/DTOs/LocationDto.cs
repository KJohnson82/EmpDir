using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.DTOs
{
    /// <summary>
    /// Data Transfer Object for Location - used for API communication
    /// </summary>
    public class LocationDto
    {
        public int Id { get; set; }
        public string LocName { get; set; } = string.Empty;
        public int? LocNum { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zipcode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Hours { get; set; }
        public int Loctype { get; set; }
        public string? AreaManager { get; set; }
        public string? StoreManager { get; set; }
        public DateTime? RecordAdd { get; set; }
        public bool Active { get; set; }

        // Flattened related data
        public string? LoctypeName { get; set; }
    }

}
