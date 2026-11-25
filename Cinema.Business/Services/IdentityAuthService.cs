using Cinema.Business.Dtos;
using Cinema.DataAccess.Repositories;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Business.Services
{
    public class IdentityAuthService(IUserRepository _userRepository)
    {
        public Task<List<User>> GetAllAsync() => _userRepository.GetAllAsync();

        public Task<User> GetProfileAsync(int id) => _userRepository.FindByIdAsync(id);

        public async Task<bool> LoginExistsAsync(string login) => await _userRepository.FindByNameAsync(login) != null;

        public async Task<bool> EmailExistsAsync(string email) => await _userRepository.FindByEmailAsync(email) != null;

        public Task<IdentityResult> RegisterAsync(User user, string password) => _userRepository.CreateAsync(user, password);

        public async Task<User> ValidateUserByEmailAsync(string email, string password)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            return user != null && await _userRepository.CheckPasswordAsync(user, password) ? user : null;
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
