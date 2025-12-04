using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using EmpDir.Core.Extensions;
using EmpDir.Core.Models;
using Newtonsoft.Json;
using EmpDir.Core.EmpDir.Core.Extensions;

namespace EmpDir.Core.Services
{
    
    /// Handles fetching and exporting of location, department, and employee data.
    /// This class provides methods to query the database for different location types
    /// and export the aggregated data to a JSON file.
    
    public class ExportData
    {
        private readonly AppDbContext _context;

        
        /// Initializes a new instance of the ExportData class.
        
        /// <param name="context">The database context to be used for data operations.</param>
        public ExportData(AppDbContext context)
        {
            _context = context;
        }

        
        /// Fetches a structured list of active locations of a specific type, including their active departments and employees.
        /// This is the primary data retrieval method, designed to be reusable for all location types.
        /// The query eagerly loads and filters related departments and employees.
        
        /// <param name="locationType">The type of location to fetch (e.g., Corporate, Plant).</param>
        /// <returns>A dictionary containing a list of fully populated Location objects.</returns>
        public async Task<Dictionary<string, List<Location>>> FetchLocationsByTypeAsync(LocationType locationType)
        {
            // Query the database for locations matching the specified type and active status.
            var locations = await _context.Locations
                .Where(l => l.Loctype == (int)locationType && l.Active == true)
                // Project the results into new Location objects to avoid over-fetching.
                .Select(l => new Location
                {
                    Id = l.Id,
                    LocName = l.LocName,
                    LocNum = l.LocNum,
                    Address = l.Address,
                    City = l.City,
                    State = l.State,
                    Zipcode = l.Zipcode,
                    PhoneNumber = l.PhoneNumber,
                    FaxNumber = l.FaxNumber,
                    Email = l.Email,
                    Hours = l.Hours,
                    Loctype = l.Loctype,
                    AreaManager = l.AreaManager,
                    StoreManager = l.StoreManager,
                    Active = l.Active,
                    // Include active departments for each location.
                    Departments = l.Departments
                        .Where(d => d.Active == true)
                        .Select(d => new Department
                        {
                            Id = d.Id,
                            DeptName = d.DeptName,
                            DeptManager = d.DeptManager,
                            DeptPhone = d.DeptPhone,
                            DeptEmail = d.DeptEmail,
                            DeptFax = d.DeptFax,
                            Location = d.Location,
                            Active = d.Active,
                            // Include active employees for each department.
                            Employees = d.Employees
                                .Where(e => e.Active == true)
                                // Sort employees alphabetically for consistent ordering.
                                .OrderBy(e => e.FirstName)
                                .ThenBy(e => e.LastName)
                                .Select(e => new Employee
                                {
                                    Id = e.Id,
                                    FirstName = e.FirstName,
                                    LastName = e.LastName,
                                    JobTitle = e.JobTitle,
                                    IsManager = e.IsManager,
                                    PhoneNumber = e.PhoneNumber,
                                    CellNumber = e.CellNumber,
                                    Extension = e.Extension,
                                    Email = e.Email,
                                    NetworkId = e.NetworkId,
                                    EmpAvatar = e.EmpAvatar,
                                    Location = e.Location,
                                    Department = e.Department,
                                    Active = e.Active,
                                }).ToList()
                        })
                        // Sort departments alphabetically.
                        .OrderBy(d => d.DeptName)
                        .ToList()
                })
                // Sort locations by number, then by name.
                .OrderBy(l => l.LocNum ?? 0)
                .ThenBy(l => l.LocName)
                .ToListAsync();

            // Wrap the results in a dictionary for a consistent JSON structure.
            return new Dictionary<string, List<Location>> { { "locations", locations } };
        }

        #region Wrapper Methods for FetchLocationsByTypeAsync

        
        /// Fetches corporate location data by calling the generic fetch method.
        
        public async Task<Dictionary<string, List<Location>>> FetchCorpDataAsync() =>
            await FetchLocationsByTypeAsync(LocationType.Corporate);

        
        /// Fetches manufacturing plant location data by calling the generic fetch method.
        
        public async Task<Dictionary<string, List<Location>>> FetchPlantDataAsync() =>
            await FetchLocationsByTypeAsync(LocationType.Plant);

        
        /// Fetches Metal Mart location data by calling the generic fetch method.
        
        public async Task<Dictionary<string, List<Location>>> FetchMMDataAsync() =>
            await FetchLocationsByTypeAsync(LocationType.MetalMart);

        
        /// Fetches Service Center location data by calling the generic fetch method.
        
        public async Task<Dictionary<string, List<Location>>> FetchSCDataAsync() =>
            await FetchLocationsByTypeAsync(LocationType.ServiceCenter);

        #endregion

        
        /// Generates a comprehensive JSON object containing data for all location types.
        /// It executes all data fetch operations in parallel to improve performance.
        
        /// <returns>A dictionary representing the final JSON structure with all location data.</returns>
        public async Task<Dictionary<string, object>> GenerateJson()
        {
            // Create an array of tasks to fetch data for all location types concurrently.
            var tasks = new[]
            {
                FetchLocationsByTypeAsync(LocationType.Corporate),
                FetchLocationsByTypeAsync(LocationType.MetalMart),
                FetchLocationsByTypeAsync(LocationType.ServiceCenter),
                FetchLocationsByTypeAsync(LocationType.Plant)
            };

            // Await the completion of all tasks.
            var results = await Task.WhenAll(tasks);

            // Assemble the final dictionary with keys corresponding to each location type.
            return new Dictionary<string, object>
            {
                { "loctype", new Dictionary<string, object>
                    {
                        { LocationType.Corporate.GetJsonKey(), results[0] },
                        { LocationType.MetalMart.GetJsonKey(), results[1] },
                        { LocationType.ServiceCenter.GetJsonKey(), results[2] },
                        { LocationType.Plant.GetJsonKey(), results[3] }
                    }
                }
            };
        }

        
        /// Exports the generated directory data to a timestamped JSON file.
        /// The file is saved to a specified directory or a default 'EMP_Data' folder in the user's Documents.
        
        /// <param name="outputDirectory">Optional. The full path of the directory to save the file in. If null, a default path is used.</param>
        /// <exception cref="Exception">Throws an exception if the JSON serialization or file writing fails.</exception>
        public async Task ExportJsonToFileAsync(string? outputDirectory = null)
        {
            try
            {
                // Generate the complete data structure.
                var jsonData = await GenerateJson();
                // Serialize the data to an indented JSON string, ignoring null values and reference loops.
                var jsonString = JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });

                // Determine the output path, using the provided directory or defaulting to a folder in MyDocuments.
                var directoryPath = outputDirectory ??
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EMP_Data");

                // Ensure the target directory exists.
                Directory.CreateDirectory(directoryPath);

                // Create a timestamped file name to avoid overwriting previous exports.
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmm");
                var fileName = $"MED_Export_{timestamp}.json";
                var filePath = Path.Combine(directoryPath, fileName);

                // Write the JSON string to the file asynchronously.
                await File.WriteAllTextAsync(filePath, jsonString);

                Console.WriteLine($"JSON Export Successful. File saved at {filePath}");
            }
            catch (Exception ex)
            {
                // Wrap and re-throw any exceptions with a more descriptive message.
                throw new Exception($"JSON export failed: {ex.Message}", ex);
            }
        }
    }
}
