using LibraryApp.Models;

namespace LibraryApp.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserRepository _userRepository;

        public UserManagementService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task ChangeUserTypeAsync(string username, string newType)
        {
            await _userRepository.ChangeUserTypeAsync(username, newType);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _userRepository.UserExistsAsync(username);
        }
    }
}
