using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using camagruApp.Models;

namespace camagruApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsersContext _context;

        public HomeController(UsersContext context)
        {
            _context = context;
        } 

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["imgid"] = 0;
            ViewData["userid"] = HttpContext.Session.GetString("userid");
            ViewData["username"] = HttpContext.Session.GetString("username");

            var model = new camagruApp.Models.ImgListData();
            model.imgList = await _context.Images.Include("Comments").ToListAsync();
            
            return View(model);
        }

        [NonAction]
         private bool IsUserLoggedIn()
        {
            var userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            return (userid != 0);
        }

        public async Task<IActionResult> Like(int id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "User");
            }
            var img = _context.Images.SingleOrDefault(x => x.id == id);
            img.likes += 1;
            _context.Update(img);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Comments(int id)
        {
            var img = _context.Images.SingleOrDefault(x => x.id == id);
            if (img.id != 0)
            {
                return View(_context.Comments.Where(x => x.id == id).ToList());
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int imgid, string content)
        {
            if (!IsUserLoggedIn())
            {
               return RedirectToAction("Login", "User");
            }
            var newcomment = new Comment();
            newcomment.dateposted = DateTime.Now;
            newcomment.imgid = imgid;
            newcomment.content = content;
            newcomment.userid = Convert.ToInt16(HttpContext.Session.GetString("userid"));
            newcomment.username = HttpContext.Session.GetString("username");
            _context.Comments.Add(newcomment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("userid", "0");
            HttpContext.Session.SetString("username", "");
            return(RedirectToAction("Index"));
        }

        [BindProperty]
        public Img Img { get; set; }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Img = await _context.Images.FindAsync(id);

            if (Img != null)
            {
                _context.Images.Remove(Img);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
