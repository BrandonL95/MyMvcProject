using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMCVProject.Models.Person;

namespace MyMCVProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LoginEmailView(string email, string password)
        {
            if (email != null)
            {
                PersonContex context = HttpContext.RequestServices.GetService(typeof(MyMCVProject.Models.Person.PersonContex)) as Models.Person.PersonContex;
                bool valid = context.validateEmail(email, password);

                if (valid)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Error", "Invalid login attempt");
                    return View();
                }
            }

            return View();
        }
    }
}