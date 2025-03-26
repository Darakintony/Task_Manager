namespace TaskManagementAPI.DTO
{
    public class RegisterUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
