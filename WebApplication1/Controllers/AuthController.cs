using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
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
        public async Task<ActionResult<List<UserProfileResponse>>> GetAll()
        {
            var users = await _identityAuthService.GetAllAsync();
            return Ok(_mapper.Map<List<UserProfileResponse>>(users));
        }

        [HttpGet("users/{id:int}")]
        public async Task<ActionResult<UserProfileResponse>> GetProfile(int id)
        {
            var user = await _identityAuthService.GetProfileAsync(id);
            if (user == null)
                return NotFound($"User with id {id} not found.");

            return Ok(_mapper.Map<UserProfileResponse>(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserProfileResponse>> Register([FromBody] IdentityRegisterRequest data)
        {
            var validationResult = _registerValidator.Validate(data);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            if (await _identityAuthService.LoginExistsAsync(data.Login))
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Login is already taken.",
                    Detail = $"Login '{data.Login}' is already taken. Choose another login."
                });

            if (await _identityAuthService.EmailExistsAsync(data.Email))
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Email is already registered.",
                    Detail = "User with this email already exists."
                });

            var user = _mapper.Map<User>(data);
            var result = await _identityAuthService.RegisterAsync(user, data.Password);
            if (!result.Succeeded)
                return IdentityFailure(result);

            return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, _mapper.Map<UserProfileResponse>(user));
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

        private ActionResult IdentityFailure(IdentityResult result)
        {
            var errors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            if (result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail"))
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Registration conflict.",
                    Detail = string.Join(" ", result.Errors.Select(e => e.Description))
                });

            return ValidationProblem(new ValidationProblemDetails(errors));
        }
    }
}
