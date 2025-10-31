
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

public class LibraryRepository
{
    private readonly LibraryDbContext _context;

    public LibraryRepository(LibraryDbContext context)
    {
        _context = context;
    }

    // Add Book
    public async Task AddBookAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    // Remove Book
    public async Task RemoveBookAsync(Book book)
    {
        if (!await IsBookStillBorrowed(book.Id))
        {
            var borrowed = _context.BorrowedBooks.Where(bb => bb.book_id == book.Id);
            _context.BorrowedBooks.RemoveRange(borrowed);
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    
    public async Task<Book?> FindBookByNameAsync(string name)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());
    }

    
    public async Task IncreaseBookQuantityAsync(string name)
    {
        var book = await FindBookByNameAsync(name);
        if (book != null)
        {
            book.Quantity += 1;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DecreaseBookQuantityAsync(string name)
    {
        var book = await FindBookByNameAsync(name);
        if (book != null && book.Quantity > 0)
        {
            book.Quantity -= 1;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Book>> GetBooksOrderedByNameAsync()
    {
        return await _context.Books
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<User?> GetUserbyIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> AddBorrowedAsync(User user, Book book)
    {
        var exists = await _context.BorrowedBooks.AnyAsync(bb =>
            bb.user_id == user.Id && bb.book_id == book.Id && !bb.IsReturned);

        if (exists)
            return false;

        var borrowedBook = new BorrowedBook
        {
            user_id = user.Id,
            book_id = book.Id,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(15),
            IsReturned = false
        };

        _context.BorrowedBooks.Add(borrowedBook);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task DeliverBorrowedAsync(User user, Book book)
    {
        var borrowed = await _context.BorrowedBooks
            .FirstOrDefaultAsync(bb => bb.user_id == user.Id && bb.book_id == book.Id && !bb.IsReturned);

        if (borrowed != null)
        {
            borrowed.IsReturned = true;
            borrowed.ReturnDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
    {
        return await _context.BorrowedBooks
            .Where(bb => bb.user_id == userId && !bb.IsReturned && bb.Book != null)
            .Include(bb => bb.Book)
            .Select(bb => bb.Book!) 
            .ToListAsync();
    }

    public async Task<List<Book>> GetReturnedBooksAsync(int userId)
    {
        return await _context.BorrowedBooks
            .Where(bb => bb.user_id == userId && bb.IsReturned && bb.Book != null)
            .Include(bb => bb.Book)
            .Select(bb => bb.Book!)
            .ToListAsync();
    }

    public async Task<List<Book>> SearchBooksByNameAsync(string keyword)
    {
        return await _context.Books
            .Where(b => EF.Functions.ILike(b.Name, $"%{keyword}%"))
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<List<User>> GetUserOrderedByNameAsync()
    {
        return await _context.Users
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<List<BorrowedBook>> GetBorrowedBooksWithUserIdAsync(int userId)
    {
        return await _context.BorrowedBooks
            .Where(bb => bb.user_id == userId)
            .ToListAsync();
    }

    public async Task<List<Book>> SearchBookByCategoryAsync(string genre)
    {
        return await _context.Books
            .Where(b => b.Genre == genre)
            .ToListAsync();
    }

    public async Task<bool> IsBookStillBorrowed(int bookId)
    {
        return await _context.BorrowedBooks
            .AnyAsync(bb => bb.book_id == bookId && !bb.IsReturned);
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books.OrderBy(b => b.Name).ToListAsync();
    }

    public async Task<List<User>> SearchUsersByUsernameAsync(string keyword)
    {
        return await _context.Users
            .Where(u => EF.Functions.ILike(u.Username, $"%{keyword}%"))
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }
    public async Task<string?> GetFullNameByUserIdAsync(int userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync();

        return user; 
    }

    public async Task<string?> GetBookNameByBookIdAsync(int bookId)
    {
        var bookName = await _context.Books
            .Where(b => b.Id == bookId)
            .Select(b => b.Name)
            .FirstOrDefaultAsync();

        return bookName; 
    }

    public async Task<string?> GetUsernameByUserIdAsync(int userId)
    {
        var username = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Username)
            .FirstOrDefaultAsync();

        return username; 
    }
    
    public async Task<List<BorrowedBookDisplay>> GetAllBorrowedBooksAsync()
    {
        return await _context.BorrowedBooks
            .Include(bb => bb.User)
            .Include(bb => bb.Book)
            .OrderByDescending(bb => bb.BorrowDate)
            .Select(bb => new BorrowedBookDisplay
            {
                FullName = bb.User!.FullName,
                Username = bb.User!.Username,
                BookName = bb.Book!.Name,
                BorrowDate = bb.BorrowDate,
                IsReturned = bb.IsReturned,
                DueDate = bb.DueDate,
                ReturnDate = bb.ReturnDate
            })
            .ToListAsync();
    }
}
