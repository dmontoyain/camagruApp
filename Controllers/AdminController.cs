using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using camagruApp.Models;

namespace camagruApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UsersContext _context;

        public AdminController(UsersContext context)
        {
            _context = context;
        }

        // GET: User
        [HttpGet]
        public IActionResult Filter()
        {
            return View();
        }

        [NonAction]
        private bool IsValidImg(string fileextension)
        {
            return (fileextension == ".jpg" || fileextension == ".jpeg" || fileextension == ".png");
        }

        [HttpPost]
        public async Task<IActionResult> Filter(IFormFile filter, [Bind("id, name")] Filter flt)
        {
            if (ModelState.IsValid)
            {
                ViewData["IsImgValid"] = "";
                if (!IsValidImg(Path.GetExtension(filter.FileName)))
                {
                    ViewData["IsImgValid"] = "Image type not supported or not valid.";
                    return View();
                }
                if (filter.Length > 0)
                {
                    Bitmap newfilter = new Bitmap(filter.FileName);
                    newfilter.MakeTransparent();
                    for (int x = 0; x < newfilter.Width; x++)
                    {
                        for (int y = 0; y < newfilter.Height; y++)
                        {
                            Color currentColor = newfilter.GetPixel(x, y);
                            if (currentColor.R >= 220 && currentColor.G >= 220 && currentColor.B >= 220)
                            {
                                newfilter.SetPixel(x, y, Color.Transparent);
                            }
                        }
                    }
                    var stream = new MemoryStream();
                    newfilter.Save(stream, ImageFormat.Bmp);
                    flt.img = stream.ToArray();
                    _context.Filters.Add(flt);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return View(flt);  
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}