using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{

    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Username == username);
    }
}