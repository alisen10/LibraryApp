using LibraryApp.Models;
namespace LibraryApp.Business
{
    public class UserService
    {
        private readonly LibraryRepository _repository;

        public UserService(LibraryRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> GetUserByIdAsync(int id) =>
            await _repository.GetUserbyIdAsync(id);

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await _repository.GetUserByUsernameAsync(username);

        public async Task<List<User>> GetUserOrderedByNameAsync() =>
            await _repository.GetUserOrderedByNameAsync();

        public async Task<List<User>> SearchUsersByUsernameAsync(string keyword) =>
            await _repository.SearchUsersByUsernameAsync(keyword);

        public async Task<string?> GetFullNameByUserIdAsync(int userId) =>
            await _repository.GetFullNameByUserIdAsync(userId);

        public async Task<string?> GetUsernameByUserIdAsync(int userId) =>
            await _repository.GetUsernameByUserIdAsync(userId);
    }
}
