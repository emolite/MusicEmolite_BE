namespace MS_Application.DataTransferObjects.FoodEmolite
{
    /// <summary>Mirrors FoodEmolite's Auth.RegisterRequest - used to create an agent account over there.</summary>
    public class CreateFoodAgentRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
