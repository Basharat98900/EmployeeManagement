using System.Collections.Generic;

namespace EF_DotNetCore.Models
{
    public class ClaimsViewModel
    {
        public string UserID { get; set; }
        public List<UserClaim> Cliam { get; set; }= new List<UserClaim>();
    }
}
