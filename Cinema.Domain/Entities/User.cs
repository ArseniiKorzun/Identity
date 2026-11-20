using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        [MaxLength(30)]
        public string DisplayName { get; set; }

        [MaxLength(60)]
        public string City { get; set; }

        [MaxLength(50)]
        public string FavoriteGenre { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
