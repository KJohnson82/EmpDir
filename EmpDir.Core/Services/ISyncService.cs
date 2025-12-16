using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDir.Core.Services
{
    /// Service for syncing data from API to local cache
    /// Implemented in EmpDir.Desktop
    public interface ISyncService
    {
        /// Perform full sync from API to cache (called on app launch)
        Task<SyncResult> SyncOnLaunchAsync();

        /// Check if API is available without syncing
        Task<bool> IsApiAvailableAsync();

        /// Get the last successful sync time
        DateTime? LastSyncTime { get; }

        /// Check if app is currently syncing
        bool IsSyncing { get; }
    }

    /// Result of a sync operation
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