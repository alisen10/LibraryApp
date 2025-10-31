namespace LibraryApp.Models
{
    public class BorrowedBook
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int book_id { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }

        public virtual User? User { get; set; }
        public virtual Book? Book { get; set; }

    }
}