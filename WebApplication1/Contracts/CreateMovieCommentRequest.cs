using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Contracts
{
    public class CreateMovieCommentRequest : IMovieCommentRequest
    {
        [Required]
        [MaxLength(100)]
        public string AuthorName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }
    }
}
