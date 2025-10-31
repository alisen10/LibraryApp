using LibraryApp.Models;


namespace LibraryApp.Business
{
    public class BorrowService
    {
        private readonly LibraryRepository _repository;

        public BorrowService(LibraryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddBorrowedAsync(User user, Book book) =>
            await _repository.AddBorrowedAsync(user, book);

        public async Task DeliverBorrowedAsync(User user, Book book) =>
            await _repository.DeliverBorrowedAsync(user, book);

        public async Task<List<Book>> GetBorrowedBooksAsync(int userId) =>
            await _repository.GetBorrowedBooksAsync(userId);

        public async Task<List<Book>> GetReturnedBooksAsync(int userId) =>
            await _repository.GetReturnedBooksAsync(userId);

        public async Task<List<BorrowedBookDisplay>> GetAllBorrowedBooksAsync() =>
            await _repository.GetAllBorrowedBooksAsync();

        public async Task<List<BorrowedBook>> GetBorrowedBooksWithUserIdAsync(int userId) =>
            await _repository.GetBorrowedBooksWithUserIdAsync(userId);
    }
}
