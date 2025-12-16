using EmpDir.Core.Models;

namespace EmpDir.Core.Services;

/// Interface for search functionality across multiple entity types.
/// Implemented by SearchService (server-side) and CachedSearchService (desktop).
public interface ISearchService
{
    /// Performs a comprehensive search across all searchable entities.
    /// <param name="searchTerm">The search term to look for</param>
    /// <returns>SearchResults containing matching Employees, Departments, and Locations</returns>
    Task<SearchResults> SearchAllAsync(string searchTerm);
}