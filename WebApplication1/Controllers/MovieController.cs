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
    public class MovieController(
        MovieService _movieService,
        MovieSessionService _sessionService,
        IValidator<CreateMovieRequest> _createValidator,
        IValidator<UpdateMovieRequest> _updateValidator,
        IMapper _mapper) : ControllerBase
    {
        [HttpGet("movies")]
        public async Task<ActionResult<List<MovieResponse>>> GetAllMovies()
        {
            var response = _mapper.Map<List<MovieResponse>>(await _movieService.GetAllAsync());
            return Ok(response);
        }

        [HttpGet("movies/search")]
        public async Task<ActionResult<List<MovieResponse>>> SearchMovies([FromQuery] MovieSearchRequest request)
        {
            var movies = await _movieService.SearchAsync(_mapper.Map<MovieSearchDto>(request));
            return Ok(_mapper.Map<List<MovieResponse>>(movies));
        }

        [HttpGet("movies/{id:int}")]
        public async Task<ActionResult<MovieResponse>> GetMovie(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
                return NotFound($"Movie with id {id} not found.");

            return Ok(_mapper.Map<MovieResponse>(movie));
        }

        [HttpPost("movies")]
        public async Task<ActionResult<MovieResponse>> AddMovie([FromBody] CreateMovieRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            var movie = _mapper.Map<Movie>(request);
            await _movieService.AddAsync(movie);
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, _mapper.Map<MovieResponse>(movie));
        }

        [HttpPut("movies/{id:int}")]
        public async Task<ActionResult<MovieResponse>> UpdateMovie(int id, [FromBody] UpdateMovieRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
                return NotFound($"Movie with id {id} not found.");

            _mapper.Map(request, movie);
            await _movieService.UpdateAsync(movie);
            return Ok(_mapper.Map<MovieResponse>(movie));
        }

        [HttpDelete("movies/{id:int}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
                return NotFound($"Movie with id {id} not found.");

            if (await _sessionService.HasUpcomingForMovieAsync(id))
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Movie has upcoming sessions.",
                    Detail = $"Movie with id {id} has upcoming sessions. Delete them before removing the movie."
                });

            await _movieService.DeleteAsync(movie);
            return NoContent();
        }

    }
}
