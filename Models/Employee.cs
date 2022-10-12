using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF_DotNetCore.Models
{
    public class Employee
    {
       
        public int? ID { get; set; }

        [Required(ErrorMessage ="Name is Required"),MaxLength(40,ErrorMessage ="Name can contain Max 40 char"),MinLength(3,ErrorMessage ="Name should have atleast 3 char")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is Required"), MaxLength(40, ErrorMessage = "Address can contain Max 40 char"), MinLength(3, ErrorMessage = "Address should have atleast 3 char")]
        public string Address { get; set; }

        [Required(ErrorMessage ="Salary is requried")]
        public int? Salary { get; set; }

        [Required(ErrorMessage ="DOB is Required")]
        public string DateOfBirth { get;  set; }

        public string PhotoPath { get; set; }

        [Required(ErrorMessage = "Please select a Department")]
        public Dept? Department { get; set; } 

   

    }

  public  enum Dept
    {
        None,
        HR,
        IT,
        Payroll
    }

   
}
