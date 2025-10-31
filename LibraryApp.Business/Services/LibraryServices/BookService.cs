using LibraryApp.Models;

namespace LibraryApp.Business
{
    public class BookService
    {
        private readonly LibraryRepository _repository;

        public BookService(LibraryRepository repository)
        {
            _repository = repository;
        }

        public async Task AddBookAsync(Book book) =>
            await _repository.AddBookAsync(book);

        public async Task RemoveBookAsync(Book book) =>
            await _repository.RemoveBookAsync(book);

        public async Task<Book?> FindBookByNameAsync(string name) =>
            await _repository.FindBookByNameAsync(name);

        public async Task IncreaseBookQuantityAsync(string name) =>
            await _repository.IncreaseBookQuantityAsync(name);

        public async Task DecreaseBookQuantityAsync(string name) =>
            await _repository.DecreaseBookQuantityAsync(name);

        public async Task<List<Book>> GetBooksOrderedByNameAsync() =>
            await _repository.GetBooksOrderedByNameAsync();

        public async Task<List<Book>> SearchBooksByNameAsync(string keyword) =>
            await _repository.SearchBooksByNameAsync(keyword);

        public async Task<List<Book>> SearchBookByCategoryAsync(string genre) =>
            await _repository.SearchBookByCategoryAsync(genre);

        public async Task<List<Book>> GetAllBooksAsync() =>
            await _repository.GetAllBooksAsync();

        public async Task<bool> IsBookStillBorrowedAsync(int bookId) =>
            await _repository.IsBookStillBorrowed(bookId);

        public async Task<string?> GetBookNameByBookIdAsync(int bookId) =>
            await _repository.GetBookNameByBookIdAsync(bookId);
    }
}
