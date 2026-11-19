using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Contracts;
using Cinema.Domain.Entities;
using Cinema.Business.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieSessionController(
        MovieSessionService _sessionService,
        MovieService _movieService,
        IValidator<CreateMovieSessionRequest> _createValidator,
        IValidator<UpdateMovieSessionRequest> _updateValidator,
        IMapper _mapper) : ControllerBase
    {
        [HttpGet("sessions")]
        public async Task<ActionResult<List<MovieSessionResponse>>> GetSessions([FromQuery] int? movieId, [FromQuery] DateOnly? date, [FromQuery] decimal? maxPrice)
        {
            if (maxPrice < 0)
                return Invalid(nameof(maxPrice), "Max price cannot be negative.");

            var sessions = await _sessionService.GetUpcomingAsync(movieId, date, maxPrice);
            return Ok(_mapper.Map<List<MovieSessionResponse>>(sessions));
        }

        [HttpGet("sessions/{id:int}")]
        public async Task<ActionResult<MovieSessionResponse>> GetSession(int id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
                return NotFound($"Session with id {id} not found.");

            return Ok(_mapper.Map<MovieSessionResponse>(session));
        }

        [HttpPost("sessions")]
        public async Task<ActionResult<MovieSessionResponse>> AddSession([FromBody] CreateMovieSessionRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            var movie = await _movieService.GetByIdAsync(request.MovieId);
            if (movie == null)
                return Invalid(nameof(request.MovieId), $"Movie with id {request.MovieId} not found.");

            var session = _mapper.Map<MovieSession>(request);
            session.Movie = movie;
            await _sessionService.AddAsync(session);
            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, _mapper.Map<MovieSessionResponse>(session));
        }

        [HttpPut("sessions/{id:int}")]
        public async Task<ActionResult<MovieSessionResponse>> UpdateSession(int id, [FromBody] UpdateMovieSessionRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
                return NotFound($"Session with id {id} not found.");

            if (request.StartsAt != session.StartsAt && request.StartsAt <= DateTime.UtcNow)
                return Invalid(nameof(request.StartsAt), "New session start time cannot be in the past.");

            var movie = await _movieService.GetByIdAsync(request.MovieId);
            if (movie == null)
                return Invalid(nameof(request.MovieId), $"Movie with id {request.MovieId} not found.");

            _mapper.Map(request, session);
            session.Movie = movie;
            await _sessionService.UpdateAsync(session);
            return Ok(_mapper.Map<MovieSessionResponse>(session));
        }

        [HttpDelete("sessions/{id:int}")]
        public async Task<ActionResult> DeleteSession(int id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
                return NotFound($"Session with id {id} not found.");

            await _sessionService.DeleteAsync(session);
            return NoContent();
        }

        private ActionResult Invalid(string field, string message) =>
            ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { [field] = [message] }));
    }
}
