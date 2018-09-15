using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestWebAp.Models.DocsViewModel;

namespace TestWebAp.Controllers
{
    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult ChatView()
        {
           DocsContextClass dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            ViewBag.email = dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

            return View();
        }
    }
}
