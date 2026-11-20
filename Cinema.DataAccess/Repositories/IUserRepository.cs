using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cinema.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> FindByIdAsync(int id);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByNameAsync(string userName);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
    }
}
