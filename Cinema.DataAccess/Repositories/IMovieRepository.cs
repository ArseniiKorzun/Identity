using System.Linq.Expressions;
using Cinema.Domain.Entities;

namespace Cinema.DataAccess.Repositories
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync();
        Task<List<Movie>> GetAsync(Expression<Func<Movie, bool>> predicate);
        Task<Movie> GetByIdAsync(int id);
        void Add(Movie movie);
        void Update(Movie movie);
        void Delete(Movie movie);
        Task SaveChangesAsync();
    }
}
