using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using EF_DotNetCore.Utilities;

namespace EF_DotNetCore.Models
{
    public class RegisterUser
    {
        [Required]
        [EmailAddress]
        [Remote("EmailInUse","Account")]
        [ValidEmailDomain(alloweddomain:"abc.com",ErrorMessage ="Provided domain is not correct use 'abc.com'")]
        public string UserName { get; set; }

        [Required]
        public Genders Gender { get; set; }

       

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password does not match")]
        public string ConfirmPassword { get; set; }

    }
}
