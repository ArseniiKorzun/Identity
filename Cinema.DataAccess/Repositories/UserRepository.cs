using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<List<User>> GetAllAsync() => _userManager.Users.ToListAsync();

        public Task<User> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

        public Task<User> FindByNameAsync(string userName) => _userManager.FindByNameAsync(userName);

        public Task<IdentityResult> CreateAsync(User user, string password) => _userManager.CreateAsync(user, password);

        public Task<bool> CheckPasswordAsync(User user, string password) => _userManager.CheckPasswordAsync(user, password);
    }
}
