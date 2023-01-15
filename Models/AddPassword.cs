using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EF_DotNetCore.Models
{
    public class AddPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string  addPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("addPassword",ErrorMessage ="Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
