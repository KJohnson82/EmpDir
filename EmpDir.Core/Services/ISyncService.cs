using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.Services
{
    /// <summary>
    /// Service for syncing data from API to local cache
    /// Implemented in EmpDir.Desktop
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Perform full sync from API to cache (called on app launch)
        /// </summary>
        Task<SyncResult> SyncOnLaunchAsync();

        /// <summary>
        /// Check if API is available without syncing
        /// </summary>
        Task<bool> IsApiAvailableAsync();

        /// <summary>
        /// Get the last successful sync time
        /// </summary>
        DateTime? LastSyncTime { get; }

        /// <summary>
        /// Check if app is currently syncing
        /// </summary>
        bool IsSyncing { get; }
    }

    /// <summary>
    /// Result of a sync operation
    /// </summary>
    public class SyncResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int EmployeesUpdated { get; set; }
        public int DepartmentsUpdated { get; set; }
        public int LocationsUpdated { get; set; }
        public int LocationTypesUpdated { get; set; }
        public DateTime SyncTime { get; set; }
        public bool ApiWasAvailable { get; set; }


    }
}