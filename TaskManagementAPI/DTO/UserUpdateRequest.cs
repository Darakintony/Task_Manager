namespace TaskManagementAPI.DTO
{
    public class UserUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }


    }
    

   
}
