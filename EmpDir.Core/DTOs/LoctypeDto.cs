using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.DTOs
{
    /// Data Transfer Object for Location Type - used for API communication
    public class LoctypeDto
    {
        public int Id { get; set; }
        public string LoctypeName { get; set; } = string.Empty;
    }
}
