using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Contracts
{
    public class UpdateMovieSessionRequest : IMovieSessionRequest
    {
        public int MovieId { get; set; }

        public DateTime StartsAt { get; set; }

        [Required]
        [MaxLength(50)]
        public string HallName { get; set; }

        [Range(0, 99999999.99)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int AvailableSeats { get; set; }
    }
}
