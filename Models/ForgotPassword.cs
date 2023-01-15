using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class ForgotPassword
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
