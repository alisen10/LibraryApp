namespace LibraryApp.Models;

using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Username { get; set; } = "";
    public string UserType { get; set; } = "";
    public string Password { get; set; } = "";

    public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
}
