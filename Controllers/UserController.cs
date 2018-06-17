using System;
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
    public class UserController : Controller
    {
        private readonly UsersContext _context;

        public UserController(UsersContext context)
        {
            _context = context;
        }

        // GET: User
        public IActionResult Index()
        {
            return View(_context.Users.ToList());
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: User Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind("id,Username,Password")] LoginViewModel loginViewModel)
        {
            ViewData["UsernameNotExists"] = null;
            ViewData["IncorrectPassword"] = null;
            if (ModelState.IsValid)
            {
                if (!LoginViewModelExists(loginViewModel.Username))
                {
                    ViewData["UsernameNotExists"] = "Username doesn't exist";
                    return View(loginViewModel);
                }
                #region Secure Password
                loginViewModel.Password = Crypto.Hash(loginViewModel.Password);
                #endregion

                if (ValidateUserPassword(loginViewModel.Username, loginViewModel.Password) == false)
                {
                    ViewData["IncorrectPassword"] = "Incorrect Password";
                    return View(loginViewModel);
                }
                loginViewModel.online = true;
                loginViewModel.id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(loginViewModel);
            }
        }


        [NonAction]
        private bool ValidateUserPassword(string username, string password)
        {
            var user = _context.Users.Where(u => u.username.Equals(username) && u.passwd.Equals(password)).FirstOrDefault();
            if (user == null)
            {
                return (false);
            }
            else
            {
                HttpContext.Session.SetString("userid", user.id.ToString());
                HttpContext.Session.SetString("username", user.username.ToString());                
                return (true);
            }
        }

        [NonAction]
        private bool LoginViewModelExists(string username)
        {
            return _context.Users.Any(e => e.username == username);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,firstname,lastname,email,username,passwd,ConfirmPassword")] User user)
        {
            bool Status = false;
            string message = "";
            ViewData["DuplicateMailError"] = null;
            if (ModelState.IsValid)
            {
                #region Email already exists
                if (DoesEmailExist(user.email))
                {
                    ViewData["DuplicateMailError"] = "Email already exists";
                    return View(user);
                }
                #endregion

                #region Generate Activation Code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                user.passwd = Crypto.Hash(user.passwd);
//                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                user.IsEmailVerified = false;

                #region Save to Database
                _context.Add(user);
                await _context.SaveChangesAsync();
                #endregion

                SendVerificationLinkEmail(user.email, user.ActivationCode.ToString());
                message = "Registration succesfully done. Account activation link " +
                    "has been sent to your email address:" + user.email;
                Status = true;
            }
            else
            {
                message = "Invalid Request";
            }
            ViewData["Message"] = message;
            ViewData["Status"] = Status;
            return RedirectToPage("./User/Login");
        }

        [NonAction]
        private void SendVerificationLinkEmail(string email, string ActivationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + ActivationCode;
            var link = "http://localhost:5000" + verifyUrl;

            var fromEmail = new MailAddress("camagruappdmontoya@gmail.com", "Account Verification");
            var toemail = new MailAddress(email);
            var fromEmailpwd = "KqozwppB";
            string subject = "CamaGru: Your account was succesfully created!";

            string body = "<br/><br/> Welcome to the Camagru App.  We are excited to have you on board!"
            + "Your account was created succesfully.  We still require one final step to verify your account" +
            " <br/><br/><a href='"+link+"'>"+link+"</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailpwd)
            };

            using (var message = new MailMessage(fromEmail, toemail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var id = Convert.ToUInt64(HttpContext.Session.GetString("userid"));

            if (id != 0)
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.id == (int)id);
                ViewData["username"] = HttpContext.Session.GetString("username");
                return (View(user));
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: User/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var id = Convert.ToUInt64(HttpContext.Session.GetString("userid"));

            var user = await _context.Users.SingleOrDefaultAsync(x => x.id == (int)id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id, email,username,passwd")] User user)
        {
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToPage("/User/Details");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(m => m.id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }

        [NonAction]
        public bool DoesEmailExist(string emailID)
        {
            return (_context.Users.Any(x => x.email == emailID));
        }
    }
}
