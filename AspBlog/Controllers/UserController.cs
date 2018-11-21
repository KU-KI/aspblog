using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspBlog.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspBlog.Controllers
{
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public UserController(ApplicationDbContext context, UserManager<IdentityUser> manager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _manager = manager;
            _signInManager = signInManager;

        }

        private async Task<IdentityUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        [Authorize]
        [Route("/profile")]
        /*
         * Returns the view where user can change their pass
        */
        public IActionResult Profile()
        {
            var user = GetCurrentUser().Result;
            return View("Profile", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        /*
        * Gets the input of change password form, checks if the user exists, checks the password confirmation, and finally changes the pass.
        */
        [Route("/profile")]
        public async Task<IActionResult> ChangePassAsync(string oldpassword, string password, string password_confirm)
        {
            if (oldpassword == null || password == null || password_confirm == null)
            {
                return RedirectToAction("Profile", "User");

            }

            var user = GetCurrentUser().Result;
            if (user == null)
            {
                return RedirectToAction("Profile", "User");
            }

            if (!password_confirm.Equals(password))
            {
                return RedirectToAction("Profile", "User");
            }

            var changePasswordResult = await _manager.ChangePasswordAsync(user, oldpassword, password);
            if (!changePasswordResult.Succeeded)
            {
                return StatusCode(500);

            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        [Route("/myposts")]
        /*
         * Gets all posts created by current user, and sorts them descending by created_at column
         */
        public IActionResult MyPosts()
        {
            ViewData["Title"] = "Moje príspevky";
            ViewData["Heading"] = ViewData["Title"];
            var user = GetCurrentUser().Result;
            var posts = _context.Post.Where(p => p.User_id.Equals(user.Id)).OrderByDescending(p => p.Created_at);
            return View("Index", posts);
        }

        [Authorize]
        [Route("/user/{username}")]
        public IActionResult GetPostsByUser(string username)
        {
            var user = _manager.FindByEmailAsync(username).Result;

            if (user == null)
            {
                return NotFound();
            }

            var posts = _context.Post.Where(p => p.User_id.Equals(user.Id)).OrderByDescending(p => p.Created_at);
            ViewData["Title"] = username;
            ViewData["Heading"] = posts.ToArray().Length + " príspevkov užívateľa \"" + username + "\"";
            return View("Index", posts);
        }
    }
}