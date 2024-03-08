using Microsoft.EntityFrameworkCore;
using Web.Client.Models;
using Web.Data;

namespace Web.Services;

public class UserService
{
    private ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser> GetApplicationUser(ApplicationUser user)
    {
        var applicationUser = await _context
            .Users.Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        return applicationUser;
    }
}
