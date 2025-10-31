namespace LibraryApp.Models;

public class BorrowedBookDisplay
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string BookName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
}
