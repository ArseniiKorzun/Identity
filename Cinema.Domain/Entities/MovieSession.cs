using System.ComponentModel.DataAnnotations;

namespace Cinema.Domain.Entities
{
    public class MovieSession
    {
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public DateTime StartsAt { get; set; }

        [Required]
        [MaxLength(50)]
        public string HallName { get; set; }

        public decimal Price { get; set; }

        public int AvailableSeats { get; set; }
    }
}
