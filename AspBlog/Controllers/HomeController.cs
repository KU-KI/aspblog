using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspBlog.Models;
using AspBlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AspBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /
        /*
         * Lists all posts, sorting them descending by the date it's created
         */
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Všetky príspevky";
            ViewData["Heading"] = "Blog";
            return View("Index", await _context.Post.OrderByDescending(p => p.Created_at).ToListAsync());
        }



    }
}
