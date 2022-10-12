using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class RoleEditModel
    {
        public RoleEditModel()
        {
            Users = new List<string>();
        }

        [Display(Name ="ID")]
        public string Id { get; set; }

        [Required]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
