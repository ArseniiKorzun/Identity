using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Contracts;
using Cinema.Domain.Entities;
using Cinema.Business.Dtos;
using Cinema.Business.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        IdentityAuthService _identityAuthService,
        IValidator<IdentityRegisterRequest> _registerValidator,
        IValidator<IdentityLoginRequest> _loginValidator,
        IMapper _mapper) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<ActionResult<List<UserResponse>>> GetAll(){

            var users = await _identityAuthService.GetAllAsync();
            var response = _mapper.Map<List<UserResponse>>(users);


            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] IdentityRegisterRequest data)
        {
            var validationResult = _registerValidator.Validate(data);

            if (!validationResult.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
            }

            if (await _identityAuthService.EmailExistsAsync(data.Email))
                return Conflict("User with this email already exists.");

            var user = _mapper.Map<User>(data);
            var result = await _identityAuthService.RegisterAsync(user, data.Password);
            if (!result.Succeeded)
                return ValidationProblem(new ValidationProblemDetails(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));
            return CreatedAtAction(nameof(GetAll), new { id = user.Id }, _mapper.Map<UserResponse>(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<bool>> Login([FromBody] IdentityLoginRequest data)
        {
            var validationResult = _loginValidator.Validate(data);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var loginDto = _mapper.Map<LoginDto>(data);
            var isValid = await _identityAuthService.ValidateUserCredentialsAsync(loginDto);
            return Ok(isValid);
        }

    }
}
