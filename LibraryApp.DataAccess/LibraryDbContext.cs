using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BorrowedBook> BorrowedBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User Mapping
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.FullName).HasColumnName("fullname");
            entity.Property(u => u.Username).HasColumnName("username");
            entity.Property(u => u.Password).HasColumnName("password");
            entity.Property(u => u.UserType).HasColumnName("user_type");

            entity.HasMany(u => u.BorrowedBooks)
                  .WithOne(bb => bb.User)
                  .HasForeignKey(bb => bb.user_id);
        });

        // Book Mapping
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Id).HasColumnName("id");
            entity.Property(b => b.Name).HasColumnName("name");
            entity.Property(b => b.Author).HasColumnName("author");
            entity.Property(b => b.Quantity).HasColumnName("quantity");
            entity.Property(b => b.Page_number).HasColumnName("page_number");
            entity.Property(b => b.Genre).HasColumnName("genre");
            entity.Property(b => b.Expression).HasColumnName("expression");

            entity.HasMany(b => b.BorrowedBooks)
                  .WithOne(bb => bb.Book)
                  .HasForeignKey(bb => bb.book_id);
        });

        modelBuilder.Entity<BorrowedBook>(entity =>
        {
            entity.ToTable("borrowed_books");
            entity.HasKey(bb => bb.Id);  

            entity.Property(bb => bb.Id).HasColumnName("id");
            entity.Property(bb => bb.user_id).HasColumnName("user_id");
            entity.Property(bb => bb.book_id).HasColumnName("book_id");
            entity.Property(bb => bb.BorrowDate).HasColumnName("borrow_date");
            entity.Property(bb => bb.DueDate).HasColumnName("due_date");
            entity.Property(bb => bb.ReturnDate).HasColumnName("return_date");
            entity.Property(bb => bb.IsReturned).HasColumnName("is_returned");
        });

        base.OnModelCreating(modelBuilder);
    }
}