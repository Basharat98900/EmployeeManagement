using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
