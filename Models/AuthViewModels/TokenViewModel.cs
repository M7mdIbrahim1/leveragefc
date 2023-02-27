namespace Backend.Models.AuthViewModels
{
    public class TokenViewModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}