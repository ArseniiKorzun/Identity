using Cinema.Domain.Entities;
using Cinema.DataAccess.Repositories;

namespace Cinema.Business.Services
{
    public class MovieSessionService(IMovieSessionRepository _sessionRepository)
    {
        public Task<List<MovieSession>> GetUpcomingAsync(int? movieId, DateOnly? date, decimal? maxPrice) =>
            _sessionRepository.GetUpcomingAsync(DateTime.UtcNow, movieId, date, maxPrice);

        public Task<MovieSession> GetByIdAsync(int id) => _sessionRepository.GetByIdAsync(id);

        public Task<bool> HasUpcomingForMovieAsync(int movieId) =>
            _sessionRepository.HasUpcomingForMovieAsync(movieId, DateTime.UtcNow);

        public async Task AddAsync(MovieSession session)
        {
            _sessionRepository.Add(session);
            await _sessionRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(MovieSession session)
        {
            _sessionRepository.Update(session);
            await _sessionRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(MovieSession session)
        {
            _sessionRepository.Delete(session);
            await _sessionRepository.SaveChangesAsync();
        }
    }
}
