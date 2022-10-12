using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class ApplicaitonUsers : IdentityUser
    {
        [Required]
        public Genders gender { get; set; }

       
    }

   public enum Genders
    {
        Male =1,
        Female =2,
        Ohters =3,
    }
}
