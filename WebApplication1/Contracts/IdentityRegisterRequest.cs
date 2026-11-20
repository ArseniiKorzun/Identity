namespace WebApplication1.Contracts
{
    public class IdentityRegisterRequest
    {
        public string Login { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string DisplayName { get; set; }

        public string City { get; set; }

        public string FavoriteGenre { get; set; }
    }
}
