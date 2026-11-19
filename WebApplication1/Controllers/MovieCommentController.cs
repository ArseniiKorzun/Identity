using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Contracts;
using Cinema.Domain.Entities;
using Cinema.Business.Services;

namespace WebApplication1.Controllers
{
    [Route("api/Movie")]
    [ApiController]
    public class MovieCommentController(
        MovieCommentService _commentService,
        IValidator<CreateMovieCommentRequest> _createValidator,
        IValidator<UpdateMovieCommentRequest> _updateValidator,
        IMapper _mapper) : ControllerBase
    {
        [HttpGet("movies/{movieId:int}/comments")]
        public async Task<ActionResult<List<MovieCommentResponse>>> GetComments(int movieId)
        {
            if (!await _commentService.MovieExistsAsync(movieId))
                return NotFound($"Movie with id {movieId} not found.");

            var response = _mapper.Map<List<MovieCommentResponse>>(await _commentService.GetByMovieIdAsync(movieId));
            return Ok(response);
        }

        [HttpPost("movies/{movieId:int}/comments")]
        public async Task<ActionResult<MovieCommentResponse>> AddComment(int movieId, [FromBody] CreateMovieCommentRequest request)
        {
            var validationResult = _createValidator.Validate(request);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            if (!await _commentService.MovieExistsAsync(movieId))
                return NotFound($"Movie with id {movieId} not found.");

            var comment = _mapper.Map<MovieComment>(request);
            comment.MovieId = movieId;
            await _commentService.AddAsync(comment);
            return CreatedAtAction(nameof(GetComments), new { movieId }, _mapper.Map<MovieCommentResponse>(comment));
        }

        [HttpPut("movies/{movieId:int}/comments/{id:int}")]
        public async Task<ActionResult<MovieCommentResponse>> UpdateComment(int movieId, int id, [FromBody] UpdateMovieCommentRequest request)
        {
            var validationResult = _updateValidator.Validate(request);
            if (!validationResult.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

            var comment = await FindComment(movieId, id);
            if (comment == null)
                return NotFound($"Comment with id {id} not found for movie {movieId}.");

            _mapper.Map(request, comment);
            await _commentService.UpdateAsync(comment);
            return Ok(_mapper.Map<MovieCommentResponse>(comment));
        }

        [HttpDelete("movies/{movieId:int}/comments/{id:int}")]
        public async Task<ActionResult> DeleteComment(int movieId, int id)
        {
            var comment = await FindComment(movieId, id);
            if (comment == null)
                return NotFound($"Comment with id {id} not found for movie {movieId}.");

            await _commentService.DeleteAsync(comment);
            return NoContent();
        }

        private async Task<MovieComment> FindComment(int movieId, int id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            return comment != null && comment.MovieId == movieId ? comment : null;
        }
    }
}
