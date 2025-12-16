using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Cryptography;
using System.Text;

namespace EmpDir.Admin.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string password);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        // Key for storing authentication state in encrypted session storage
        private const string AuthKey = "IsAuthenticated";

        public AuthService(ProtectedSessionStorage sessionStorage, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _sessionStorage = sessionStorage;
            _configuration = configuration;
            _logger = logger;
        }

        // Verify password against stored hash and create authenticated session
        public async Task<bool> LoginAsync(string password)
        {
            try
            {
                _logger.LogInformation("Login Attempt Started");

                // Get stored password hash from appsettings.json
                var storedPasswordHash = _configuration["AppSettings:PasswordHash"];
                var enteredPasswordHash = HashPassword(password);

                // Compare hashes - never compare plain text passwords
                if (enteredPasswordHash == storedPasswordHash)
                {
                    await _sessionStorage.SetAsync(AuthKey, true);
                    _logger.LogInformation("Login successful");
                    return true;
                }

                _logger.LogWarning("Login failed - invalid password");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Exception");
                _logger.LogError(ex, "Login failed due to exception");
                return false;
            }
        }

        // Clear authenticated session
        public async Task LogoutAsync()
        {
            try
            {
                await _sessionStorage.DeleteAsync(AuthKey);
                _logger.LogInformation("User logged out");
            }
            catch
            {
                _logger.LogWarning("Error during logout - session may already be cleared");
            }
        }

        // Check if user has valid authenticated session
        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<bool>(AuthKey);
                return result.Success && result.Value;
            }
            catch
            {
                // Session storage unavailable (prerendering or error)
                return false;
            }
        }

        // Convert plain text password to SHA256 hash for secure comparison
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}