using System.Linq.Expressions;
using Cinema.Business.Dtos;
using Cinema.Domain.Entities;
using Cinema.DataAccess.Repositories;

namespace Cinema.Business.Services
{
    public class MovieService(IMovieRepository _movieRepository)
    {
        public Task<List<Movie>> GetAllAsync() => _movieRepository.GetAllAsync();

        public Task<List<Movie>> SearchAsync(MovieSearchDto criteria) =>
            _movieRepository.GetAsync(BuildSearchPredicate(criteria));

        public Task<Movie> GetByIdAsync(int id) => _movieRepository.GetByIdAsync(id);

        public async Task AddAsync(Movie movie)
        {
            _movieRepository.Add(movie);
            await _movieRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _movieRepository.Update(movie);
            await _movieRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Movie movie)
        {
            _movieRepository.Delete(movie);
            await _movieRepository.SaveChangesAsync();
        }

        private static Expression<Func<Movie, bool>> BuildSearchPredicate(MovieSearchDto c)
        {
            var m = Expression.Parameter(typeof(Movie), "m");
            Expression body = Expression.Constant(true);
            void And(Expression condition) => body = Expression.AndAlso(body, condition);
            Expression Prop(string name) => Expression.Property(m, name);
            Expression Contains(string name, string value) =>
                Expression.Call(Prop(name), nameof(string.Contains), null, Expression.Constant(value));

            if (c.Id is int id)
                And(Expression.Equal(Prop(nameof(Movie.Id)), Expression.Constant(id)));
            if (!string.IsNullOrWhiteSpace(c.Name))
                And(Contains(nameof(Movie.Title), c.Name));
            if (!string.IsNullOrWhiteSpace(c.Genre))
                And(Expression.Equal(Prop(nameof(Movie.Genre)), Expression.Constant(c.Genre)));
            if (c.ReleaseYear is int year)
                And(Expression.Equal(Prop(nameof(Movie.ReleaseYear)), Expression.Constant(year)));
            if (!string.IsNullOrWhiteSpace(c.Country))
                And(Expression.Equal(Prop(nameof(Movie.Country)), Expression.Constant(c.Country)));
            if (!string.IsNullOrWhiteSpace(c.Keyword))
                And(Expression.OrElse(Contains(nameof(Movie.Title), c.Keyword), Contains(nameof(Movie.Description), c.Keyword)));

            return Expression.Lambda<Func<Movie, bool>>(body, m);
        }
    }
}
