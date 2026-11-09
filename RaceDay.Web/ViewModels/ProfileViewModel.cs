
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }
    }

}

