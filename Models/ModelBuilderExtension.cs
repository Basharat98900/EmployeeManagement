
using Microsoft.EntityFrameworkCore;
namespace EF_DotNetCore.Models
{
    public static class ModelBuilderExtension 
    {
        public static void Seed(this ModelBuilder obj)
        {
            obj.Entity<Employee>().HasData(
                new Employee
                {
                    ID = 1,
                    Address = "Chawni",
                    //DateOfBirth = System.DateTime.Now,
                    Name = "Mike",
                    Salary = 3000
                }
                ) ;
        }
    }
}
