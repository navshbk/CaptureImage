using CaptureImage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CaptureImage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string name)
        {
            var files = HttpContext.Request.Form.Files;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Getting Filename  
                        var fileName = file.FileName;
                        // Unique filename "Guid"  
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        // Getting Extension  
                        var fileExtension = Path.GetExtension(fileName);
                        // Concating filename + fileExtension (unique filename)  
                        var newFileName = string.Concat(myUniqueFileName, fileExtension);
                        //  Generating Path to store photo   
                        var filepath = Path.Combine(_environment.WebRootPath, "CameraImages") + $@"\{newFileName}";

                        if (!string.IsNullOrEmpty(filepath))
                        {
                            // Storing Image in Folder  
                            StoreInFolder(file, filepath);
                        }

                        var imageBytes = System.IO.File.ReadAllBytes(filepath);
                        if (imageBytes != null)
                        {
                            // Storing Image in Folder  
                            //StoreInDatabase(imageBytes);
                        }

                    }
                }
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public IActionResult LoadCapturedImages()
        {
            var allImages = GetCapturedImages();
            return PartialView("~/Views/Home/_CapturedImages.cshtml", allImages);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void StoreInFolder(IFormFile file, string fileName)
        {
            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }

        private List<CapturedImagesModel> GetCapturedImages()
        {
            List<CapturedImagesModel> allImages = new List<CapturedImagesModel>();

            string[] filePaths = Directory.GetFiles(Path.Combine(_environment.WebRootPath, "CameraImages/"));

            int id = 1;
            foreach (string filePath in filePaths)
            {
                allImages.Add(new CapturedImagesModel { Path = Path.GetFileName(filePath), Id = id.ToString() });
                id++;
            }
            return allImages;
        }

        public FileResult DownloadFile(string Name)
        {
            string FilePath = Path.Combine(_environment.WebRootPath, "DownloadImages");

            string FileNameWithPath = Path.Combine(FilePath, Name);

            byte[] bytes = System.IO.File.ReadAllBytes(FileNameWithPath);

            return File(bytes, "application/octet-stream", Name);

        }
    }
}