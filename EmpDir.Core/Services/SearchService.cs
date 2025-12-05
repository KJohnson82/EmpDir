using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using EmpDir.Core.Models;
using Telerik.DataSource.Extensions;

namespace EmpDir.Core.Services;


/// Service class that provides comprehensive search functionality across multiple entity types.
/// Searches through Employees, Departments, and Locations using case-insensitive string matching.
/// Uses Entity Framework Core for data access and includes extensive debugging capabilities.

public class SearchService : ISearchService
{
    #region Fields

    
    /// Entity Framework database context for accessing application data.
    /// Injected via dependency injection to enable testability and proper lifetime management.
    
    private readonly AppDbContext _context;

    #endregion

    #region Constructor

    
    /// Initializes a new instance of the SearchService with the provided database context.
    
    /// <param name="context">The Entity Framework database context</param>
    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    #endregion

    #region Public Methods

    
    /// Performs a comprehensive search across all searchable entities (Employees, Departments, Locations).
    /// Returns empty results for null/whitespace search terms to prevent unnecessary database queries.
    /// Includes extensive error handling and debug logging for troubleshooting.
    
    /// <param name="searchTerm">The search term to look for across all entities</param>
    /// <returns>A SearchResults object containing all matching entities organized by type</returns>
    public async Task<SearchResults> SearchAllAsync(string searchTerm)
    {
        // Return early if search term is empty/null to avoid unnecessary database calls
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new SearchResults();

        Console.WriteLine($"DEBUG: Starting search for term: '{searchTerm}'");

        try
        {
            // Execute searches across all entity types in parallel for better performance
            var employees = await SearchEmployeesAsync(searchTerm);
            var departments = await SearchDepartmentsAsync(searchTerm);
            var locations = await SearchLocationsAsync(searchTerm);

            Console.WriteLine($"DEBUG: Found {employees.Count} employees, {departments.Count} departments, {locations.Count} locations");

            // Aggregate results into a single response object
            return new SearchResults
            {
                Employees = employees,
                Departments = departments,
                Locations = locations,
                SearchTerm = searchTerm
            };
        }
        catch (Exception ex)
        {
            // Log error and return empty results with search term preserved for user feedback
            Console.WriteLine($"DEBUG: Search error: {ex.Message}");
            return new SearchResults { SearchTerm = searchTerm };
        }
    }

    #endregion

    #region Private Search Methods

    
    /// Searches for employees matching the provided search term.
    /// Searches across multiple employee fields: name, job title, contact information.
    /// Uses in-memory filtering after loading active employees to avoid complex SQL generation.
    /// Limited to 20 results for performance and UI considerations.
    
    /// <param name="searchTerm">The term to search for in employee data</param>
    /// <returns>List of matching Employee entities with related data included</returns>
    private async Task<List<Employee>> SearchEmployeesAsync(string searchTerm)
    {
        try
        {
            Console.WriteLine($"DEBUG: Searching employees for '{searchTerm}'");

            // Load all active employees with related data (Department and Location)
            // Include() ensures related entities are loaded to prevent N+1 query issues
            var allEmployees = await _context.Employees
                .Include(e => e.EmpDepartment)    // Load department information
                .Include(e => e.EmpLocation)      // Load location information
                .Where(e => e.Active == true)     // Only include active employees
                .ToListAsync();

            Console.WriteLine($"DEBUG: Total active employees in database: {allEmployees.Count}");

            // Early return if no employees exist to prevent null reference issues
            if (!allEmployees.Any())
            {
                Console.WriteLine("DEBUG: No active employees found in database!");
                return new List<Employee>();
            }

            // Perform case-insensitive search across multiple employee fields
            // Using in-memory LINQ to avoid complex SQL generation issues
            var filteredEmployees = allEmployees.Where(e =>
                e.FirstName != null && e.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.LastName != null && e.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.JobTitle != null && e.JobTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.PhoneNumber != null && e.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.CellNumber != null && e.CellNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Email != null && e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Extension != null && e.Extension.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).Take(20).ToList(); // Limit results to prevent UI performance issues

            Console.WriteLine($"DEBUG: Found {filteredEmployees.Count} matching employees using simple LINQ");

            return filteredEmployees;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: Employee search error: {ex.Message}");
            return new List<Employee>(); // Return empty list on error to prevent application crashes
        }
    }

    
    /// Searches for departments matching the provided search term.
    /// Searches across department name, manager, and contact information.
    /// Uses the same in-memory filtering pattern as employee search for consistency.
    
    /// <param name="searchTerm">The term to search for in department data</param>
    /// <returns>List of matching Department entities with location data included</returns>
    private async Task<List<Department>> SearchDepartmentsAsync(string searchTerm)
    {
        try
        {
            Console.WriteLine($"DEBUG: Searching departments for '{searchTerm}'");

            // Load all active departments with related location data
            var allDepartments = await _context.Departments
                .Include(d => d.DeptLocation)     // Load location information for departments
                .Where(d => d.Active == true)     // Only include active departments
                .ToListAsync();

            Console.WriteLine($"DEBUG: Total active departments in database: {allDepartments.Count}");

            // Search across department-specific fields with null safety checks
            var filteredDepartments = allDepartments.Where(d =>
                d.DeptName != null && d.DeptName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                d.DeptManager != null && d.DeptManager.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                d.DeptPhone != null && d.DeptPhone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                d.DeptEmail != null && d.DeptEmail.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).Take(20).ToList(); // Limit results for performance

            Console.WriteLine($"DEBUG: Found {filteredDepartments.Count} matching departments");

            return filteredDepartments;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: Department search error: {ex.Message}");
            return new List<Department>();
        }
    }

    
    /// Searches for locations matching the provided search term.
    /// Searches across the most comprehensive set of fields including address, management, and location number.
    /// Includes special handling for numeric location number searches.
    
    /// <param name="searchTerm">The term to search for in location data</param>
    /// <returns>List of matching Location entities with type information included</returns>
    private async Task<List<Location>> SearchLocationsAsync(string searchTerm)
    {
        try
        {
            Console.WriteLine($"DEBUG: Searching locations for '{searchTerm}'");

            // Load all active locations with location type information
            var allLocations = await _context.Locations
                .Include(l => l.LocationType)     // Load location type for categorization
                .Where(l => l.Active == true)     // Only include active locations
                .ToListAsync();

            Console.WriteLine($"DEBUG: Total active locations in database: {allLocations.Count}");

            // Most comprehensive search covering all location-related fields
            var filteredLocations = allLocations.Where(l =>
                l.LocName != null && l.LocName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.Address != null && l.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.City != null && l.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.State != null && l.State.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.Zipcode != null && l.Zipcode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.PhoneNumber != null && l.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.AreaManager != null && l.AreaManager.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                l.StoreManager != null && l.StoreManager.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                // Special handling for numeric location number search
                l.LocNum.HasValue && l.LocNum.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).Take(20).ToList(); // Limit results for performance

            Console.WriteLine($"DEBUG: Found {filteredLocations.Count} matching locations");

            return filteredLocations;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: Location search error: {ex.Message}");
            return new List<Location>();
        }
    }

    #endregion
}


/// Data transfer object that aggregates search results from multiple entity types.
/// Provides convenient properties for result analysis and UI binding.
/// Initialized with empty collections to prevent null reference exceptions.

public class SearchResults
{
    
    /// Collection of Employee entities that matched the search criteria.
    /// Initialized as empty list to prevent null reference issues.
    
    public List<Employee> Employees { get; set; } = new();

    
    /// Collection of Department entities that matched the search criteria.
    /// Initialized as empty list to prevent null reference issues.
    
    public List<Department> Departments { get; set; } = new();

    
    /// Collection of Location entities that matched the search criteria.
    /// Initialized as empty list to prevent null reference issues.
    
    public List<Location> Locations { get; set; } = new();

    
    /// The original search term that was used to generate these results.
    /// Useful for display purposes and debugging.
    
    public string SearchTerm { get; set; } = "";

    
    /// Computed property that returns the total number of results across all entity types.
    /// Useful for displaying result counts and pagination logic.
    
    public int TotalResults => Employees.Count + Departments.Count + Locations.Count;

    
    /// Computed property that indicates whether any results were found.
    /// Convenient for conditional UI rendering (show/hide no results messages).
    
    public bool HasResults => TotalResults > 0;
}