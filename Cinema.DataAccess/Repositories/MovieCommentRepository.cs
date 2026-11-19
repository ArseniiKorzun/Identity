using Microsoft.EntityFrameworkCore;
using Cinema.DataAccess.Context;
using Cinema.Domain.Entities;

namespace Cinema.DataAccess.Repositories
{
    public class MovieCommentRepository : IMovieCommentRepository
    {
        private readonly AppDbContext _context;

        public MovieCommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<MovieComment>> GetByMovieIdAsync(int movieId) =>
            _context.MovieComments.Where(c => c.MovieId == movieId).OrderBy(c => c.CreatedAt).ToListAsync();

        public Task<MovieComment> GetByIdAsync(int id) => _context.MovieComments.FirstOrDefaultAsync(c => c.Id == id);

        public void Add(MovieComment comment) => _context.MovieComments.Add(comment);

        public void Update(MovieComment comment) => _context.MovieComments.Update(comment);

        public void Delete(MovieComment comment) => _context.MovieComments.Remove(comment);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
