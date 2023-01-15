using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
