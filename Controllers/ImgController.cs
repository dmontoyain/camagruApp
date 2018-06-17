using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using camagruApp.Models;

namespace camagruApp.Controllers
{
    public class ImgController : Controller
    {
        private readonly UsersContext _context;

        public ImgController(UsersContext context)
        {
            _context = context;
        }

        [NonAction]
        private bool IsUserLoggedIn()
        {
            var userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            return (userid != 0);
        }

        [NonAction]
        private bool IsValidImg(string fileextension)
        {
            var userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            return (fileextension == ".jpg" || fileextension == ".jpeg" || fileextension == ".png");
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile img, [Bind("id,caption")] Img image)
        {
            if (ModelState.IsValid)
            {
                ViewData["IsImgValid"] = "";
                if (!IsValidImg(Path.GetExtension(img.FileName)))
                {
                    ViewData["IsImgValid"] = "Image type not supported or not valid.";
                    return View();
                }
                if (img.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        img.CopyTo(stream);
                        var filename = img.FileName;
                        image.img = stream.ToArray();
                    }

                   image.userid = Convert.ToInt16(HttpContext.Session.GetString("userid"));
                   image.username = HttpContext.Session.GetString("username");
                   image.dateposted = DateTime.Now;
                   _context.Images.Add(image);
                   await _context.SaveChangesAsync();
                }
                else
                {
                    return View(image);  
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Capture()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "User");
            }
            ViewData["ImgCaptured"] = 0;
            ViewData["Message"] = "Upload your favorite pictures or why not... Create one!";
            return View();
        }

        [HttpPost]
        public ActionResult Capture([Bind ("id, caption")] Img image)
        {
            var stream = Request.Body;
            string dump;

            using (var reader = new StreamReader(stream))
            {
                dump = reader.ReadToEnd();
            }
            ViewData["ImgCaptured"] = 1;
            image.img = String_To_Bytes2(dump);
            return View(image);
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }
    }
}