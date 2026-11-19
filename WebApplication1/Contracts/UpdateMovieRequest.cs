namespace WebApplication1.Contracts
{
    public class UpdateMovieRequest : IMovieRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string PhotoPath { get; set; }

        public string Director { get; set; }

        public int Runtime { get; set; }

        public int ReleaseYear { get; set; }

        public string PosterUrl { get; set; }

        public string MovieUrl { get; set; }

        public string Genre { get; set; }

        public string Country { get; set; }
    }
}
