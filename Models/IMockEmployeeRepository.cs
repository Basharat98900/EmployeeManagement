using System.Collections.Generic;

namespace EF_DotNetCore.Models
{
    public interface IMockEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployees();
        Employee Add(Employee obj);
        void Delete(Employee obj);
        void Update(Employee obj);
        public Employee getEmployeeWithID(int? ID);
    }
}
