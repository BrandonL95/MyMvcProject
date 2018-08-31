using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyMCVProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LoginEmailView()
        {
            return View();
        }

        public IActionResult LoginPasswordView()
        {
            return View();
        }
    }
}