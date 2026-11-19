using Cinema.Domain.Entities;

namespace Cinema.DataAccess.Repositories
{
    public interface IMovieSessionRepository
    {
        Task<List<MovieSession>> GetUpcomingAsync(DateTime now, int? movieId, DateOnly? date, decimal? maxPrice);
        Task<MovieSession> GetByIdAsync(int id);
        Task<bool> HasUpcomingForMovieAsync(int movieId, DateTime now);
        void Add(MovieSession session);
        void Update(MovieSession session);
        void Delete(MovieSession session);
        Task SaveChangesAsync();
    }
}
