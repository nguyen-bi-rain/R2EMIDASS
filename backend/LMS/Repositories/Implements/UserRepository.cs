using LMS.Data;
using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repositories.Implements
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<User> GetUserByRefreshToken(string refreshToken)
        {
                return _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null && user.Role != null)
            {
                return user.Role.Name;
            }
            return string.Empty;
        }

        
    }
}