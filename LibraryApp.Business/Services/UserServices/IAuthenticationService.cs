namespace LibraryApp.Services
{
    public interface IAuthenticationService
    {
        Task<int> LogInAsync(string username, string password);
        Task SignUpAsync(string fullName, string username, string password, string userType);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    }
}


