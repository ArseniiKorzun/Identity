using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Cinema.DataAccess.Context;
using Cinema.Domain.Entities;

namespace Cinema.DataAccess.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context;

        public MovieRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Movie movie) => _context.Movies.Add(movie);

        public void Remove(Movie movie) => _context.Movies.Remove(movie);

        public void Update(Movie movie) => _context.Movies.Update(movie);

        public void Delete(Movie movie) => _context.Movies.Remove(movie);

        public Task<List<Movie>> GetAllAsync() => _context.Movies.ToListAsync();

        public Task<List<Movie>> GetAsync(Expression<Func<Movie, bool>> predicate) => _context.Movies.Where(predicate).ToListAsync();

        public Task<Movie> GetByIdAsync(int id) => _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
