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
using System.Security.Cryptography;

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
        public async Task<IActionResult> UploadFilePrivate(IFormFile file)
        {
            try
            {
                dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

                if (file == null || file.Length == 0)
                    return Content("file not selected");

                string path = "C:\\Users\\brand\\Desktop\\Userfiles\\" + file.FileName;

                if (dbContext.AddPrivateDocs(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(), file.FileName.ToString(), path))
                {
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                    return View("UploadFile");
                }
                else
                    return View("UploadFailed");
            }
            catch (Exception)
            {
                return View("UploadFailed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFilePublic(IFormFile file)
        {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = "C:\\Users\\brand\\Desktop\\Userfiles\\" + file.FileName;

            string md5Hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty); ;
                }

                if (dbContext.AddPublicDocs(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(), file.FileName.ToString(), path.ToString(), md5Hash))
                    return View("UploadFile");
                else
                    return View("UploadFailed");
            }
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = "C:\\Users\\brand\\Desktop\\Userfiles\\" + filename;

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
                {".csv", "text/csv"}
            };
        }

        [Authorize]
        public IActionResult MyFilesView()
        {
            return View();
        }

        public IActionResult PublicDocsView()
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            return View(context.GetAllFiles());
        }

        public IActionResult PrivateDocsView()
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            return View(context.GetAllPrivateFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()));
        }

        public IActionResult UploadPublicView()
        {
            return View();
        }

        public IActionResult UploadPrivateView()
        {
            return View();
        }

        public IActionResult AddCollaberatorView(string filename)
        {
            Models.AccountViewModels.UserDBContext context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.AccountViewModels.UserDBContext)) as Models.AccountViewModels.UserDBContext;

            ViewBag.Filename = filename;

            return View("AddCollaberatorView", context.GetAllUsers());
        }

        public IActionResult WriteColaberatorFile(string email, string filename)
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            context.writeColaberatorFile(email, filename);

            return View("PrivateDocsView", context.GetAllPrivateFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()));
        }

        public IActionResult SharedView()
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            return View(context.GetSharedFiles(context.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())));
        }

    }
}