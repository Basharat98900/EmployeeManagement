using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EF_DotNetCore.Models
{
    public class Login_Google
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string returnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
