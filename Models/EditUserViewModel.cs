using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class EditUserViewModel
    {

        public EditUserViewModel()
        {
            Roles = new List<string>();
            Claims = new List<string>();    
        }
        public string ID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public Genders gender { get; set; }
        public IList<string> Roles { get; set; }

        public List<string> Claims { get; set; }
    }
}
