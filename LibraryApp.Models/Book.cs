namespace LibraryApp.Models;

public class Book
{
    public int Id;
    public string Name { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Expression { get; set; }
    public int Page_number { get; set; }

    public int Quantity { get; set; }


    public Book(int id, string name, string author, string genre, string expression, int page_number, int quantity)
    {
        Id = id;
        Name = name;
        Author = author;
        Genre = genre;
        Expression = expression;
        Page_number = page_number;
        Quantity = quantity;
    }

    public virtual ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

}