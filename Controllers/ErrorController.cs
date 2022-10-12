using Microsoft.AspNetCore.Mvc;

namespace EF_DotNetCore.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMsg = "Resource request could not be found";
                    break;

                case 500:
                    ViewBag.ErrorMsg = "Resource request could not be found";
                    break;

                default:
                    ViewBag.ErrorMsg="Request cannot proceed further";
                    break;
            }
            return View("NotFound");
        }

        public IActionResult Sample()
        {
            ViewBag.ErrorMsg = "From Sampe Action";
            return View("NotFound");
        }
        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}
