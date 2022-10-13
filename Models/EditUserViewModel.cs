using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class EditUserViewModel
    {
        public string ID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public Genders gender { get; set; }
        public List<string> Roles { get; set; }

        public List<string> Claims { get; set; }
    }
}
