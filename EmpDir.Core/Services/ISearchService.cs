using EmpDir.Core.Models;

namespace EmpDir.Core.Services;

/// <summary>
/// Interface for search functionality across multiple entity types.
/// Implemented by SearchService (server-side) and CachedSearchService (desktop).
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Performs a comprehensive search across all searchable entities.
    /// </summary>
    /// <param name="searchTerm">The search term to look for</param>
    /// <returns>SearchResults containing matching Employees, Departments, and Locations</returns>
    Task<SearchResults> SearchAllAsync(string searchTerm);
}