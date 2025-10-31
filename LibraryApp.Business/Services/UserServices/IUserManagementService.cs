using LibraryApp.Models;

namespace LibraryApp.Services
{
    public interface IUserManagementService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task ChangeUserTypeAsync(string username, string newType);
        Task<bool> UserExistsAsync(string username);
        

    }
}