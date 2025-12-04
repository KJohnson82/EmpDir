// ===================================================================
// REQUIRED NAMESPACES
// ===================================================================
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage; // For encrypted browser session storage
using System.Security.Cryptography; // For SHA256 password hashing
using System.Text; // For converting strings to bytes for hashing

namespace EmpDir.Admin.Services
{
    // ===================================================================
    // INTERFACE - Defines the contract for authentication services
    // ===================================================================
    // This interface allows us to swap implementations and makes testing easier
    public interface IAuthService
    {
        // Attempts to log in with a password, returns true if successful
        Task<bool> LoginAsync(string password);

        // Logs out the current user by clearing their session
        Task LogoutAsync();

        // Checks if the current user is authenticated (logged in)
        Task<bool> IsAuthenticatedAsync();
    }

    // ===================================================================
    // AUTHENTICATION SERVICE - Handles login, logout, and auth checks
    // ===================================================================
    public class AuthService : IAuthService
    {
        // ===================================================================
        // PRIVATE FIELDS - Dependencies and constants
        // ===================================================================

        // Encrypted browser session storage - stores auth state securely in the browser
        // Data is encrypted and only available during the current browser session
        private readonly ProtectedSessionStorage _sessionStorage;

        // Configuration service - allows us to read from appsettings.json
        private readonly IConfiguration _configuration;

        private readonly ILogger<AuthService> _logger;

        // Key used to store authentication state in session storage
        // This is the "key" in the key-value pair stored in the browser
        private const string AuthKey = "IsAuthenticated";

        // ===================================================================
        // CONSTRUCTOR - Dependency injection
        // ===================================================================
        // ASP.NET Core automatically provides these dependencies when creating the service
        public AuthService(ProtectedSessionStorage sessionStorage, IConfiguration configuration)
        {
            _sessionStorage = sessionStorage;
            _configuration = configuration;
            //_logger = logger;
        }

        // ===================================================================
        // LOGIN METHOD - Verifies password and creates authenticated session
        // ===================================================================
        public async Task<bool> LoginAsync(string password)
        {
            try
            {
                // DEBUG: These Console.WriteLine statements are for debugging
                // They can be removed in production for cleaner logs
                //Console.WriteLine($"=== LOGIN DEBUG START ===");
                //Console.WriteLine($"Password received: '{password}'");
                //Console.WriteLine($"Password length: {password.Length}");

                // ---------------------------------------------------------------
                // STEP 1: Get the stored password hash from appsettings.json
                // ---------------------------------------------------------------
                // We never store plain text passwords - only the hash
                // The hash is stored in appsettings.json under AppSettings:PasswordHash
                var storedPasswordHash = _configuration["AppSettings:PasswordHash"];

                //Console.WriteLine($"Stored hash from config: '{storedPasswordHash}'");
                //Console.WriteLine($"Stored hash is null/empty: {string.IsNullOrEmpty(storedPasswordHash)}");

                // If no password hash is configured, use a default for first-time setup
                // In production, you should always have a hash configured
                if (string.IsNullOrEmpty(storedPasswordHash))
                {
                    storedPasswordHash = HashPassword("McElr0y!"); // Default password
                    Console.WriteLine($"Using default hash: '{storedPasswordHash}'");
                }

                // ---------------------------------------------------------------
                // STEP 2: Hash the password the user entered
                // ---------------------------------------------------------------
                // We hash the entered password using the same algorithm (SHA256)
                // Then compare it to the stored hash
                var enteredPasswordHash = HashPassword(password);
                Console.WriteLine($"Entered password hash: '{enteredPasswordHash}'");

                // ---------------------------------------------------------------
                // STEP 3: Compare the hashes
                // ---------------------------------------------------------------
                Console.WriteLine($"Hashes match: {enteredPasswordHash == storedPasswordHash}");
                Console.WriteLine($"=== LOGIN DEBUG END ===");

                // If the hashes match, the password is correct
                if (enteredPasswordHash == storedPasswordHash)
                {
                    // Store authentication state in encrypted session storage
                    // This marks the user as logged in for this browser session
                    await _sessionStorage.SetAsync(AuthKey, true);
                    return true; // Login successful
                }

                return false; // Login failed - wrong password
            }
            catch (Exception ex)
            {
                // If anything goes wrong, log it and return false (login failed)
                Console.WriteLine($"=== LOGIN EXCEPTION: {ex.Message} ===");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        // ===================================================================
        // LOGOUT METHOD - Clears the authenticated session
        // ===================================================================
        public async Task LogoutAsync()
        {
            try
            {
                // Remove the authentication flag from session storage
                // This logs the user out - they'll need to login again
                await _sessionStorage.DeleteAsync(AuthKey);
            }
            catch
            {
                // If deletion fails, we ignore it since the user is logging out anyway
                // The session will be cleared when the browser closes
            }
        }

        // ===================================================================
        // AUTHENTICATION CHECK - Returns true if user is logged in
        // ===================================================================
        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                // Try to read the authentication flag from session storage
                var result = await _sessionStorage.GetAsync<bool>(AuthKey);

                // Return true only if we successfully retrieved the value AND it's true
                // result.Success = whether we found the key in storage
                // result.Value = the actual boolean value stored
                return result.Success && result.Value;
            }
            catch
            {
                // If we can't read from session storage (prerendering or storage unavailable)
                // Consider the user not authenticated
                return false;
            }
        }

        // ===================================================================
        // PASSWORD HASHING - Converts plain text password to secure hash
        // ===================================================================
        // This is a private helper method - only used internally by this class
        private string HashPassword(string password)
        {
            // Create a SHA256 hasher (secure one-way encryption)
            using var sha256 = SHA256.Create();

            // Convert the password string to bytes (required for hashing)
            var bytes = Encoding.UTF8.GetBytes(password);

            // Compute the hash - this creates a unique fingerprint of the password
            // The hash is always the same for the same input, but you can't reverse it
            var hash = sha256.ComputeHash(bytes);

            // Convert the hash bytes to a Base64 string for easy storage
            var hashString = Convert.ToBase64String(hash);

            // DEBUG: Log the input and output (remove in production)
            Console.WriteLine($"HashPassword input: '{password}' -> output: '{hashString}'");

            return hashString;
        }
    }
}

// ===================================================================
// HOW IT ALL WORKS TOGETHER
// ===================================================================
/*
 * SECURITY FLOW:
 * 
 * 1. PASSWORD STORAGE:
 *    - Plain text password: "admin123"
 *    - Stored in appsettings.json: "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk="
 *    - We NEVER store the actual password, only the hash
 * 
 * 2. LOGIN PROCESS:
 *    - User enters password: "admin123"
 *    - We hash it: "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk="
 *    - We compare hashes (not passwords)
 *    - If match: Store "IsAuthenticated = true" in session storage
 * 
 * 3. CHECKING AUTHENTICATION:
 *    - Read "IsAuthenticated" from session storage
 *    - If true: User is logged in
 *    - If false/missing: User needs to login
 * 
 * 4. SESSION STORAGE:
 *    - Encrypted by ASP.NET Core
 *    - Only exists during browser session
 *    - Automatically cleared when browser closes
 *    - Cannot be accessed from other tabs/windows
 * 
 * 5. WHY SHA256 HASHING:
 *    - One-way encryption - can't reverse the hash to get the password
 *    - Same input always produces same hash (allows comparison)
 *    - Different input produces completely different hash
 *    - Even a tiny change in password creates a completely different hash
 * 
 * EXAMPLE:
 * Password: "admin123" -> Hash: "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk="
 * Password: "admin124" -> Hash: "xjQs8F7K3m2nP9qR5tU8wV1yZ3aB6cD4eF7gH9iJ0kL=" (completely different!)
 */