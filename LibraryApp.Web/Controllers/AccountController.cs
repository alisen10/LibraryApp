using Microsoft.AspNetCore.Mvc;
using LibraryApp.Services;

namespace LibraryApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserManagementService _userManagementService;
        private readonly LibraryRepository _library;

        public AccountController(IAuthenticationService authService, IUserManagementService userManagementService, LibraryRepository library)
        {
            _authService = authService;
            _userManagementService = userManagementService;
            _library = library;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            int userId = await _authService.LogInAsync(username, password);

            if (userId > 0)
            {
                var user = await _userManagementService.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("userId", user.Id);
                    HttpContext.Session.SetString("username", user.Username);
                    HttpContext.Session.SetString("user_type", user.UserType);

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Username or password is wrong.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string fullName, string username, string password, string userType)
        {
            userType = "Library User";

            if (!await _userManagementService.UserExistsAsync(username))
            {
                await _authService.SignUpAsync(fullName, username, password, userType);
                return RedirectToAction("Login");
            }

            ViewBag.Error = "This username already exists.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _library.GetUserbyIdAsync(userId.Value);
            var borrowedBooks = await _library.GetBorrowedBooksAsync(userId.Value);
            var returnedBooks = await _library.GetReturnedBooksAsync(userId.Value);

            ViewBag.BorrowedBooks = borrowedBooks;
            ViewBag.ReturnedBooks = returnedBooks;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserType(string username, string newType)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newType))
            {
                TempData["UserTypeError"] = "Invalid input.";
                return RedirectToAction("UserTable", "Library");
            }

            await _userManagementService.ChangeUserTypeAsync(username, newType);
            TempData["UserTypeSuccess"] = $"User type of {username} has been changed to {newType}.";

            return RedirectToAction("UserTable", "Library");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ChangePasswordError"] = "You must be logged in.";
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string newPasswordConfirm)
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ChangePasswordError"] = "You must be logged in.";
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword != newPasswordConfirm)
            {
                ModelState.AddModelError("", "New passwords do not match.");
                return View();
            }

            bool success = await _authService.ChangePasswordAsync(username, currentPassword, newPassword);

            if (!success)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View();
            }

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }
    }
}
