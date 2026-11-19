using System.ComponentModel.DataAnnotations;

namespace Cinema.Domain.Entities
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

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

        public List<MovieComment> Comments { get; set; } = new();

        public List<MovieSession> Sessions { get; set; } = new();
    }
}
