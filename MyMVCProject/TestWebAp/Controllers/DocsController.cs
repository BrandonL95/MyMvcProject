using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using TestWebAp.Models.DocsViewModel;
using TestWebAp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TestWebAp.Controllers
{
    public class DocsController : Controller
    {
        DocsContextClass dbContext;

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult DocsView()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();

            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "FilesFolder", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            dbContext.AddDocs(userId, file.FileName.ToString(), path.ToString());

            return View();
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "FilesFolder", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [Authorize]
        public IActionResult MyFilesView()
        {
            //(default connection found in appsettings.json)
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            return View(context.GetAllFiles());
        }
    }
}