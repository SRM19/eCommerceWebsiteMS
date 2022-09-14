using System.ComponentModel.DataAnnotations;

namespace Foody.Services.Identity.MainModule.Account
{
    public class RegisterInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string ReturnUrl { get; set; }
        public string RoleName { get; set; }
    }
}
