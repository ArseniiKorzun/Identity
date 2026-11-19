namespace WebApplication1.Contracts
{
    public class MovieSearchRequest
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        public int? ReleaseYear { get; set; }

        public string Country { get; set; }

        public string Keyword { get; set; }
    }
}
