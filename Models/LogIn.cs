using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    
    public class LogIn
    {
        [Required(ErrorMessage ="UserName is Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Password is Required")]
        public string Password { get; set; }


    }

}
