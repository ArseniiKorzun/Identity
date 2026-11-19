using Cinema.Business.Dtos;
using Cinema.DataAccess.Repositories;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Business.Services
{
    public class IdentityAuthService(IUserRepository _userRepository)
    {
        public Task<List<User>> GetAllAsync() => _userRepository.GetAllAsync();

        public async Task<bool> EmailExistsAsync(string email) => await _userRepository.FindByEmailAsync(email) != null;

        public Task<IdentityResult> RegisterAsync(User user, string password)
        {
            user.UserName = user.Email;
            return _userRepository.CreateAsync(user, password);
        }

        public async Task<bool> ValidateUserCredentialsAsync(LoginDto loginDto)
        {
            var user = await _userRepository.FindByNameAsync(loginDto.Login);
            if (user == null)
                return false;
            return await _userRepository.CheckPasswordAsync(user, loginDto.Password);
        }
    }
}
