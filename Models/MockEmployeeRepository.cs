using System.Collections.Generic;

namespace EF_DotNetCore.Models
{
    public class MockEmployeeRepository 
    {   
        private List<Employee> _employees;

        public MockEmployeeRepository()
        {
            _employees = new List<Employee>()
            {
                //new Employee() {ID=1, Name="Jhon",Address="China",DateOfBirth=System.DateTime.Now,Salary=4000},
                //new Employee() {ID=2,Name="Marry",Address="Nepal",DateOfBirth=System.DateTime.Now,Salary=3000},
                //new Employee() {ID=2,Name="Tom",Address="USA",DateOfBirth=System.DateTime.Now,Salary=5000}
            };
        }
        public Employee GetEmployeeWithID(int ID)
        {
            Employee employee = new Employee();
            return employee;
        }
        public IEnumerable<Employee> GetAllEmployees()
        {
            
            return _employees;
        }

        public Employee Add(Employee obj)
        {
            _employees.Add(obj);
            return obj;
        }
        public void Delete(Employee obj)
        {
           
        }
        public void Update(Employee employee)
        {
            employee.Salary = 0;
        }
    }
}
