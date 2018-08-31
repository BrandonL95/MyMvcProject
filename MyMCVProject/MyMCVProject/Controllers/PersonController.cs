using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyMCVProject.Models.Person;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyMCVProject.Controllers
{
    public class PersonController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            //(default connection found in appsettings.json)
            Models.Person.PersonContex context = HttpContext.RequestServices.GetService(typeof(MyMCVProject.Models.Person.PersonContex)) as Models.Person.PersonContex;
            
            return View(context.GetAllPeople());
        }

    }
}
