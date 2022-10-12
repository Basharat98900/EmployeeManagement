using System.Collections.Generic;
using System.Linq;

namespace EF_DotNetCore.Models
{
    public class SQLEmployeeRepository : IMockEmployeeRepository
    {
        EmployessDBContext employessDBContext;
        
        public SQLEmployeeRepository(EmployessDBContext obj)
        {
            employessDBContext = obj;
        }
        
        public IEnumerable<Employee> GetAllEmployees()
        {
            return employessDBContext.Employees;
        }
        public Employee getEmployeeWithID(int? ID)
        {
            Employee employee = new Employee();
            employee=employessDBContext.Employees.Where(x => x.ID == ID).FirstOrDefault();
            return employee;    
        }

        public Employee Add(Employee obj)
        {
            employessDBContext.Add(obj);
            employessDBContext.SaveChanges();
            return obj;
        }

        public void Delete(Employee obj)
        {
            employessDBContext.Remove(obj);
            employessDBContext.SaveChanges();
        }

        public void Update(Employee updatedValues)
        {
            var employees = employessDBContext.Employees.Attach(updatedValues);
            employees.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            employessDBContext.SaveChanges();
        }
    }
}
