using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository
{
    private readonly LibraryDbContext _context;

    public UserRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username);
    }

    public async Task ChangeUserTypeAsync(string username, string newType)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user != null)
        {
            user.UserType = newType;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePasswordAsync(string username, string newHashedPassword)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user != null)
        {
            user.Password = newHashedPassword;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string?> GetHashedPasswordAsync(string username)
    {
        var user = await _context.Users
            .Where(u => u.Username == username)
            .Select(u => u.Password)
            .FirstOrDefaultAsync();

        return user;
    }
}
