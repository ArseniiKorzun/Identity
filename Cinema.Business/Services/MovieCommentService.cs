using Cinema.Domain.Entities;
using Cinema.DataAccess.Repositories;

namespace Cinema.Business.Services
{
    public class MovieCommentService(IMovieCommentRepository _commentRepository, MovieService _movieService)
    {
        public async Task<bool> MovieExistsAsync(int movieId) => await _movieService.GetByIdAsync(movieId) != null;

        public Task<List<MovieComment>> GetByMovieIdAsync(int movieId) => _commentRepository.GetByMovieIdAsync(movieId);

        public Task<MovieComment> GetByIdAsync(int id) => _commentRepository.GetByIdAsync(id);

        public async Task AddAsync(MovieComment comment)
        {
            _commentRepository.Add(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(MovieComment comment)
        {
            _commentRepository.Update(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(MovieComment comment)
        {
            _commentRepository.Delete(comment);
            await _commentRepository.SaveChangesAsync();
        }
    }
}
