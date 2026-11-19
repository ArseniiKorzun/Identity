namespace WebApplication1.Contracts
{
    public class IdentityLoginRequest
    {
        //Логін користувача, який використовується для аутентифікації.
        public string Login { get; set; }

        public string Password { get; set; }
    }
}
