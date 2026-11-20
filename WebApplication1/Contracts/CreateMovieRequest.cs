using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Contracts
{
    public class CreateMovieRequest : IMovieRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string PhotoPath { get; set; }

        [Required]
        [MaxLength(100)]
        public string Director { get; set; }

        [Range(1, 600)]
        public int Runtime { get; set; }

        public int ReleaseYear { get; set; }

        [MaxLength(500)]
        public string PosterUrl { get; set; }

        [MaxLength(500)]
        public string MovieUrl { get; set; }

        [MaxLength(50)]
        public string Genre { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }
    }
}
