using Microsoft.EntityFrameworkCore;
using Cinema.DataAccess.Context;
using Cinema.Domain.Entities;

namespace Cinema.DataAccess.Repositories
{
    public class MovieSessionRepository : IMovieSessionRepository
    {
        private readonly AppDbContext _context;

        public MovieSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<MovieSession>> GetUpcomingAsync(DateTime now, int? movieId, DateOnly? date, decimal? maxPrice)
        {
            var query = _context.MovieSessions.Include(s => s.Movie).Where(s => s.StartsAt > now);

            if (movieId is int id)
                query = query.Where(s => s.MovieId == id);

            if (date is DateOnly day)
            {
                var from = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                var to = from.AddDays(1);
                query = query.Where(s => s.StartsAt >= from && s.StartsAt < to);
            }

            if (maxPrice is decimal price)
                query = query.Where(s => s.Price <= price);

            return query.OrderBy(s => s.StartsAt).ToListAsync();
        }

        public Task<MovieSession> GetByIdAsync(int id) =>
            _context.MovieSessions.Include(s => s.Movie).FirstOrDefaultAsync(s => s.Id == id);

        public Task<bool> HasUpcomingForMovieAsync(int movieId, DateTime now) =>
            _context.MovieSessions.AnyAsync(s => s.MovieId == movieId && s.StartsAt > now);

        public void Add(MovieSession session) => _context.MovieSessions.Add(session);

        public void Update(MovieSession session) => _context.MovieSessions.Update(session);

        public void Delete(MovieSession session) => _context.MovieSessions.Remove(session);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
