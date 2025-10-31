using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LibraryApp.Models;
using LibraryApp.Business;

namespace LibraryApp.Controllers
{
    public class LibraryController : Controller
    {
        private readonly BookService _bookService;
        private readonly UserService _userService;
        private readonly BorrowService _borrowService;

        public LibraryController(BookService bookService, UserService userService, BorrowService borrowService)
        {
            _bookService = bookService;
            _userService = userService;
            _borrowService = borrowService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                context.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index(string search, string genre, int page = 1)
        {
            int pageSize = 25;
            List<Book> books;

            if (!string.IsNullOrEmpty(search))
            {
                books = await _bookService.SearchBooksByNameAsync(search);
            }
            else if (!string.IsNullOrEmpty(genre))
            {
                books = await _bookService.SearchBookByCategoryAsync(genre);
            }
            else
            {
                books = await _bookService.GetBooksOrderedByNameAsync();
            }

            int totalBooks = books.Count;
            var pagedBooks = books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.SelectedGenre = genre;

            return View(pagedBooks);
        }

        public async Task<IActionResult> UserTable(string search)
        {
            List<User> users = string.IsNullOrEmpty(search)
                ? await _userService.GetUserOrderedByNameAsync()
                : await _userService.SearchUsersByUsernameAsync(search);

            var groupedUsers = users
                .GroupBy(u => u.UserType)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(groupedUsers);
        }

        [HttpGet]
        public IActionResult AddBook() => View();

        [HttpPost]
        public async Task<IActionResult> AddBook(int id, string name, string author, string genre, string expression, int page_number, int quantity)
        {
            var book = new Book(id, name, author, genre, expression, page_number, quantity);
            await _bookService.AddBookAsync(book);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RemoveBook() => View();

        [HttpPost]
        public async Task<IActionResult> RemoveBook(string name)
        {
            var book = await _bookService.FindBookByNameAsync(name);
            if (book != null && !await _bookService.IsBookStillBorrowedAsync(book.Id))
            {
                await _bookService.RemoveBookAsync(book);
            }
            else
            {
                TempData["CannotRemoved"] = "This book is borrowed and cannot be deleted";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(string name)
        {
            await _bookService.IncreaseBookQuantityAsync(name);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(string name)
        {
            await _bookService.DecreaseBookQuantityAsync(name);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeliverBook(string bookName)
        {
            var userId = HttpContext.Session.GetInt32("userId");

            if (userId == null)
            {
                TempData["DeliverBookError"] = "You must be logged in to return a book.";
                return RedirectToAction("Index");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            var book = await _bookService.FindBookByNameAsync(bookName);

            if (user == null || book == null)
            {
                TempData["DeliverBookError"] = "User or book not found.";
                return RedirectToAction("Index");
            }

            await _borrowService.DeliverBorrowedAsync(user, book);
            await _bookService.IncreaseBookQuantityAsync(bookName);

            TempData["ReturnedSuccess"] = $"\"{book.Name}\" has been successfully returned.";

            return RedirectToAction("Profile", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(string bookName)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                TempData["SessionError"] = "Session could not be retrieved. Please log in.";
                return RedirectToAction("Index");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            var book = await _bookService.FindBookByNameAsync(bookName);

            if (user == null || book == null)
            {
                TempData["UserOrBookError"] = "User or book not found.";
                return RedirectToAction("Index");
            }

            if (book.Quantity <= 0)
            {
                TempData["BookError"] = "This book is currently out of stock.";
                return RedirectToAction("Index");
            }

            await _bookService.DecreaseBookQuantityAsync(bookName);

            var result = await _borrowService.AddBorrowedAsync(user, book);

            if (!result)
            {
                TempData["BorrowError"] = "Failed to borrow the book.";
                await _bookService.IncreaseBookQuantityAsync(bookName);
            }
            else
            {
                TempData["BorrowSuccess"] = $"\"{book.Name}\" has been successfully borrowed.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BorrowedBooks()
        {
            var borrowedBooks = await _borrowService.GetAllBorrowedBooksAsync();
            return View(borrowedBooks);
        }

        [HttpGet]
        public async Task<IActionResult> LoanBook(string bookName)
        {
            var book = await _bookService.FindBookByNameAsync(bookName);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index");
            }

            return View("LoanBook", book);
        }

        [HttpPost]
        public async Task<IActionResult> LoanBook(string bookName, string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            var book = await _bookService.FindBookByNameAsync(bookName);
            var userType = user?.UserType ?? "";

            if (userType == "Library User")
            {
                if (user == null || book == null)
                {
                    TempData["UserOrBookNotFoundError"] = "User or book not found.";
                    return View("LoanBook", book);
                }

                var result = await _borrowService.AddBorrowedAsync(user, book);
                if (!result)
                {
                    await _bookService.IncreaseBookQuantityAsync(bookName);
                    TempData["Error"] = "Failed to loan the book.";
                    return View("LoanBook", book);
                }
                else
                {
                    TempData["Success"] = $"\"{book.Name}\" successfully loaned to {username}.";
                    return View("LoanBook", book);
                }
            }
            else
            {
                TempData["UserTypeError"] = "Only Library Users can borrow book from library.";
                return View("LoanBook", book);
            }
        }
    }
}
