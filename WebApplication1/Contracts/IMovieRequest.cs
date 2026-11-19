namespace WebApplication1.Contracts
{
    public interface IMovieRequest
    {
        string Title { get; }
        string Description { get; }
        string PhotoPath { get; }
        string Director { get; }
        int Runtime { get; }
        int ReleaseYear { get; }
        string PosterUrl { get; }
        string MovieUrl { get; }
        string Genre { get; }
        string Country { get; }
    }
}
