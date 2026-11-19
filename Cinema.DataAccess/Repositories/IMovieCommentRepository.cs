using Cinema.Domain.Entities;

namespace Cinema.DataAccess.Repositories
{
    public interface IMovieCommentRepository
    {
        Task<List<MovieComment>> GetByMovieIdAsync(int movieId);
        Task<MovieComment> GetByIdAsync(int id);
        void Add(MovieComment comment);
        void Update(MovieComment comment);
        void Delete(MovieComment comment);
        Task SaveChangesAsync();
    }
}
