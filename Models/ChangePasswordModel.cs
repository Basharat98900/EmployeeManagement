﻿using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
