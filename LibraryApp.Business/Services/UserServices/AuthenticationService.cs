using LibraryApp.Models;

namespace LibraryApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserRepository _userRepository;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<int> LogInAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return -1;

            var hashedPassword = await _userRepository.GetHashedPasswordAsync(username);
            if (hashedPassword == null) return -1;

            if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                return user.Id;

            return -1;
        }

        public async Task SignUpAsync(string fullName, string username, string password, string userType)
        {
            if (await _userRepository.UserExistsAsync(username))
                throw new Exception("Username already exists!");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                FullName = fullName,
                Username = username,
                Password = hashedPassword,
                UserType = userType
            };

            await _userRepository.AddUserAsync(newUser);
        }

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var hashedPasswordInDb = await _userRepository.GetHashedPasswordAsync(username);
            if (hashedPasswordInDb == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, hashedPasswordInDb))
                return false;

            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdatePasswordAsync(username, newHashedPassword);

            return true;
        }
    }
}
