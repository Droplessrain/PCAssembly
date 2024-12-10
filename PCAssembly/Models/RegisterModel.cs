using System.ComponentModel.DataAnnotations;

namespace PCAssembly.Models
{
    public class RegisterModel
    {
        [Required]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public class RegisterInputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}