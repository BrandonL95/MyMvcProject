﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using TestWebAp.Models.DocsViewModel;
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
        public async Task<IActionResult> UploadFilePrivate(IFormFile file, string OwnerID)
        {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (file == null || file.Length == 0)
                return View("UploadFailed");

            string path = @"C:\\Users\\brand\\Desktop\\Userfiles\\" + dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()) + @"\\" + file.FileName;

            string md5Hash;
            using (var md5 = MD5.Create())
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty); ;
                }
            }

                if (dbContext.AddPrivateDocs(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(), file.FileName.ToString(), path.ToString(), md5Hash, DateTime.Now, (double)file.Length))
                    return View("UploadFile");
                else
                    return View("UploadFailed");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFilePublic(IFormFile file)
        {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (file == null || file.Length == 0)
                return View("UploadFailed");

            string path = @"C:\\Users\\brand\\Desktop\\Userfiles\\" + dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()) + @"\\" + file.FileName;

            if (dbContext.AddPublicDocs(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(), file.FileName.ToString(), path.ToString(), "fg", DateTime.Now, (double)file.Length))
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return View("UploadFile");
            }
            else
                return View("UploadFailed");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFilePublic(IFormFile file, string OwnerID, string OldFileName)
        {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (file == null || file.Length == 0)
                return View("UploadFailed");

            string path = @"C:\\Users\\brand\\Desktop\\Userfiles\\" + dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()) + @"\\" + file.FileName;

            if (dbContext.updatePublicFile(OwnerID.ToString(), OldFileName, file.FileName, (double)file.Length))
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return View("UploadFile");
            }
            else
                return View("UploadFailed");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFilePrivate(IFormFile file, string OwnerID, string OldFileName)
        {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (file == null || file.Length == 0)
                return View("UploadFailed");

            string path = @"C:\\Users\\brand\\Desktop\\Userfiles\\" + dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()) + @"\\" + file.FileName;

            if (dbContext.updatePrivateFile(OwnerID.ToString(), OldFileName, file.FileName, (double)file.Length))
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return View("UploadFile");
            }
            else
                return View("UploadFailed");
        }

        public async Task<IActionResult> Download(string filename)
            {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (filename == null)
                return Content("filename not present");

            string path = "C:\\Users\\brand\\Desktop\\Userfiles\\" + dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()) + "\\" + filename;

            if (path != null)
            {
                MemoryStream memory = new MemoryStream();
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }

            return View("SharedView");
        }

        public async Task<IActionResult> Downloadshared(string filename)
        {
            dbContext = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (filename == null)
                return Content("filename not present");

            var path = dbContext.getPath(dbContext.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()), filename);

            if (path != null)
            {
                MemoryStream memory = new MemoryStream();
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }

            return View("SharedView");
        }

        private string GetContentType(string path)
        {
            Dictionary<string, string> types = GetMimeTypes();
            string ext = Path.GetExtension(path).ToLowerInvariant();
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

            ViewBag.UserID = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();

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

            context.writeColaberatorFile(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(), email, filename);

            return View("PrivateDocsView", context.GetAllPrivateFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()));
        }

        public IActionResult SharedView()
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            return View(context.GetSharedFiles(context.GetEmail(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())));
        }

        public IActionResult DeletePrivateFile(string OwnerID, string filename, string path)
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (context.DeletePrivateFile(OwnerID, filename))
            {
                System.IO.File.Delete(path);
                return View("PrivateDocsView", context.GetAllPrivateFiles(this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString()));
            }
            else
                return View();
        }

        public IActionResult DeletePublicFile(string OwnerID, string filename, string path)
        {
            Models.DocsViewModel.DocsContextClass context = HttpContext.RequestServices.GetService(typeof(TestWebAp.Models.DocsViewModel.DocsContextClass)) as Models.DocsViewModel.DocsContextClass;

            if (context.DeletePublicFile(OwnerID, filename))
            {
                System.IO.File.Delete(path);
                return View("PublicDocsView", context.GetAllFiles());
            }
            else
                return View();
        }

        public IActionResult UpdatePublicFile(string OwnerID, string OldFileName)
        {
            DocsClass file = new DocsClass {
                OwnerID = OwnerID,
                Myfilenames = OldFileName
            };

            return View(file);
        }

        public IActionResult UpdatePrivateFile(string OwnerID, string OldFileName)
        {
            DocsClass file = new DocsClass
            {
                OwnerID = OwnerID,
                Myfilenames = OldFileName
            };

            return View(file);
        }

    }
}