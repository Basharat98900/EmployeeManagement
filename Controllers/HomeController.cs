using Microsoft.AspNetCore.Mvc;
using EF_DotNetCore.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting.Internal;
using System.Linq;
using Microsoft.CodeAnalysis;
using EF_DotNetCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using EF_DotNetCore.Security;

namespace EF_DotNetCore.Controllers
{
    public class HomeController : Controller
    {   
        private IMockEmployeeRepository _employeeRepository;

        private IWebHostEnvironment _hostingEnvironment;

        private IDataProtector protector;
        public HomeController(IMockEmployeeRepository employeeRepository,
            IWebHostEnvironment hostingEnvironment,
            DataProtectionPurposeStrings dataProtectionPurposeStrings,
            IDataProtectionProvider dataProtectionProvider)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
            protector=dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings._dataProtectionPurposeStrings);
        }

        public int unEncryptionID(string id)
        {
            string unEncryptID=protector.Unprotect(id);
            return Convert.ToInt32(unEncryptID);
        }
        public IActionResult Details(string ID)
        {
            Employee emp = _employeeRepository.getEmployeeWithID(unEncryptionID(ID));
            if(emp == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",unEncryptionID(ID));
            }
            return View(emp);
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            string unEncryptId=protector.Unprotect(id);
            int ID = Convert.ToInt32(unEncryptId);
            Employee emp = _employeeRepository.getEmployeeWithID(ID);
            UpdateEmployee updEmp = new UpdateEmployee();
            updEmp.ID = emp.ID;
            updEmp.Name = emp.Name;
            updEmp.Address = emp.Address;
            updEmp.Salary = emp.Salary;
            updEmp.DateOfBirth = emp.DateOfBirth;
            updEmp.PhotoPath = emp.PhotoPath;
            updEmp.Department = emp.Department;
            return View(updEmp);
        }
        [HttpPost]
        public IActionResult Update(UpdateEmployee obj,string ID)
        {
            Employee emp = _employeeRepository.getEmployeeWithID(unEncryptionID(ID));
            {
                emp.Name = obj.Name;
                emp.Address = obj.Address;
                emp.Salary = obj.Salary;
                emp.DateOfBirth = obj.DateOfBirth;
                emp.PhotoPath = obj.PhotoPath;
                emp.Department = obj.Department;

            };
            if(obj.Photo!=null)
            {
               
                if(obj.PhotoPath!=null)
                {
                    string filePath=Path.Combine(_hostingEnvironment.WebRootPath,"Images",obj.PhotoPath);
                    System.IO.File.Delete(filePath);
                }
                emp.PhotoPath = ProcessUplodedFile(obj);
            }
           

            _employeeRepository.Update(emp);
            return RedirectToAction("Contact");
        }
        public IActionResult Delete(string ID)
        {
            IEnumerable<Employee> obj = _employeeRepository.GetAllEmployees().Where(empid=>empid.ID==unEncryptionID(ID));
            Employee emp=obj.First();
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", emp.PhotoPath);
            System.IO.File.Delete(filePath);
            _employeeRepository.Delete(emp);
            return RedirectToAction("Contact");
        }
        public IActionResult Create()
        {
            return View();
            
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel createViewModel)
        { 
            if (ModelState.IsValid)
            {
                // If the Photo property on the incoming model object is not null, then the user
                // has selected an image to upload.
                Employee employee = new Employee();
                employee.Name = createViewModel.Name;
                employee.Department = createViewModel.Department;
                employee.Address = createViewModel.Address;
                employee.PhotoPath = ProcessUplodedFile(createViewModel);
                employee.DateOfBirth = createViewModel.DateOfBirth;
                employee.Salary = createViewModel.Salary;
                _employeeRepository.Add(employee);
            } 
                return RedirectToAction("Contact");
        }

        public void ImageResize(IFormFile file,string imagePath)
        {
            int width = 499;
            int height = 498;

            Image image = Image.FromStream(file.OpenReadStream(),true,true); 
            var newImage = new Bitmap(width,height);
            using(var a = Graphics.FromImage(newImage))
            {
                a.DrawImage(image, 0,0,width,height);
                newImage.Save(imagePath);
            }
        }

        public string ProcessUplodedFile(CreateViewModel obj)
        {
            string uniqueFileName;
            if (obj.Photo != null)
            {
                // The image must be uploaded to the images folder in wwwroot
                // To get the path of the wwwroot folder we are using the inject
                // HostingEnvironment service provided by ASP.NET Core
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
                // To make sure the file name is unique we are appending a new
                // GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + obj.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                ImageResize(obj.Photo, filePath);
                //obj.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            else
            {
                uniqueFileName = null;
            }
            return uniqueFileName;
        }
        public IActionResult Employees()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Employees(Employee obj)
        {
            _employeeRepository.Add(obj);
            return View();

        }
       
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            IEnumerable<Employee> obj= _employeeRepository.GetAllEmployees().Select(e=>
            {
                e.EncryptedId = protector.Protect(e.ID.ToString());
                return e;
            });
            return View(obj);
        }

        [HttpGet]
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        
        public IActionResult Register(Employee obj)
        {

            if (ModelState.IsValid)
            {
                var emp = obj.DateOfBirth;
                _employeeRepository.Add(obj);
                IEnumerable<Employee> obj1 = _employeeRepository.GetAllEmployees();
                return RedirectToAction("Contact");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult LogIn(LogIn obj)
        {
           if(ModelState.IsValid)
            {
                return RedirectToAction("Index", obj);
            }
            else
            {
                return View();
            }
        }
        public IActionResult LogIn()
        {
            return View();
        }

        
    }
}
