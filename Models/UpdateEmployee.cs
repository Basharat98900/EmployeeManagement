using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class UpdateEmployee : CreateViewModel
    {
        
        public string PhotoPath { get; set; }


    }
}

