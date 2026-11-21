using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Contracts
{
    public class UpdateMovieRequest : IMovieRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [MaxLength(500, ErrorMessage = "Photo path cannot exceed 500 characters.")]
        public string PhotoPath { get; set; }

        [Required(ErrorMessage = "Director is required.")]
        [MaxLength(100, ErrorMessage = "Director cannot exceed 100 characters.")]
        public string Director { get; set; }

        [Range(1, 600, ErrorMessage = "Runtime must be between 1 and 600 minutes.")]
        public int Runtime { get; set; }

        public int ReleaseYear { get; set; }

        [MaxLength(500, ErrorMessage = "Poster URL cannot exceed 500 characters.")]
        public string PosterUrl { get; set; }

        [MaxLength(500, ErrorMessage = "Movie URL cannot exceed 500 characters.")]
        public string MovieUrl { get; set; }

        [MaxLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
        public string Genre { get; set; }

        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; }
    }
}
