using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestWebAp.Models.LogsViewModel;

namespace TestWebAp.Controllers
{ 
    public class LogController : Controller
    {
        Log LogContext;

        public IActionResult DocLogView()
        {
            LogContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.LogsViewModel.Log)) as Models.LogsViewModel.Log;

            return View(LogContext.GetLog(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()));
        }
    }
}
