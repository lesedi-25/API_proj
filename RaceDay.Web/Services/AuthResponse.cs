namespace RaceDay.Web.Services
{
    public class AuthResponse
    {
        public string? Message { get; set; }

        public AuthUser? User { get; set; }
    }

    public class AuthUser
    {
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}

