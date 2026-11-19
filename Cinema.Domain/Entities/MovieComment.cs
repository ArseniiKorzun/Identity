using System.ComponentModel.DataAnnotations;

namespace Cinema.Domain.Entities
{
    public class MovieComment
    {
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        [Required]
        [MaxLength(100)]
        public string AuthorName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
