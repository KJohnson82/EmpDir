using EmpDir.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.Services
{
    /// <summary>
    /// Service interface for interacting with the Employee Directory API
    /// </summary>
    /// 
    public interface IApiService
    {
        /// Get full directory data from API for sync
        Task<DirectorySyncDto?> GetFullDirectoryAsync();

        /// Check if API is reachable and healthy
        Task<bool> HealthCheckAsync();

        /// Get the base API URL
        string GetApiUrl();
    }
}
