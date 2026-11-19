using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
