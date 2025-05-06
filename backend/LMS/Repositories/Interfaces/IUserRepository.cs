using LMS.Models.Enitties;
using LMS.Repositories.Base;

namespace LMS.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<string> GetUserRoleAsync(Guid userId);
    Task<User> GetUserByUserName(string userName);
    Task<User> GetUserByRefreshToken(string refreshToken);
}