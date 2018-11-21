using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspBlog.Data;
using AspBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AspBlog.Helpers;

namespace AspBlog.Controllers
{
    public class PostsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;

        public PostsController(ApplicationDbContext context, UserManager<IdentityUser> manager)
        {
            _context = context;
            _manager = manager;
        }

        private async Task<IdentityUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }


        // GET: post/lorem-ipsum
        [Authorize]
        [Route("post/{slug}")]
        /*
         * Gets the post, according to slug in URL
         * If the slug doesn't belong to any of posts, shows 404.
         */
        public async Task<IActionResult> Show(string slug)
        {
            if (slug.Equals(null))
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Slug.Equals(slug.ToString()));

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }



        [Route("post/create")]
        [Authorize]
        // GET: post/Create
        /*
         * Returns Create view
         */
        public IActionResult Create()
        {
            return View();
        }



        // POST: post/create
        /*
         * Gets the Title, Text from View,
         * generates slug from title 
         * and sets actual time as created at
         * and current user as author of the newly created post
         */
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("post/create")]
        public async Task<IActionResult> Create([Bind("Id,User_id,Title,Text,Slug,Created_at")] Post post)
        {

            if (ModelState.IsValid)
            {
                var user_id = (await GetCurrentUser()).Id;
                post.Created_at = DateTime.Now;
                post.User_id = user_id;
                post.Slug = StringHelper.Slugify(StringHelper.WordLimiter(post.Title, 4));
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(post);
        }

        // GET: post/slug/edit
        /*
         * Returns the edit view, checks if the slug exists in any of posts, then checks for authorization - only the author of the post can access edit view of it
         */
        [Authorize]
        [Route("post/{slug}/edit")]
        public async Task<IActionResult> Edit(string slug)
        {
            if (slug.Equals(null))
            {
                return NotFound();
            }

            var post = await _context.Post.FirstOrDefaultAsync(m => m.Slug.Equals(slug.ToString()));

            if (post == null)
            {
                return NotFound();
            }

            if (!(await GetCurrentUser()).Id.Equals(post.User_id))
            {
                return StatusCode(403);
            }

            return View(post);
        }

        // POST: post/slug/edit
        /*
        * Edits the post, checks if the slug exists in any of posts, then checks for authorization - only the author of the post can edit it
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("post/{slug}/edit")]
        public async Task<IActionResult> Edit(string slug, [Bind("Title,Text,Slug")] Post Inputpost)
        {

            if (!(await GetCurrentUser()).Id.Equals(Inputpost.User_id))
            {
                return StatusCode(403);
            }

            if (!slug.Equals(Inputpost.Slug.ToString()))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var post = _context.Post.Where(p => p.Slug.Equals(Inputpost.Slug)).FirstOrDefault();

                    if (post == null)
                    {
                        NotFound();
                    }

                    post.Slug = StringHelper.Slugify(StringHelper.WordLimiter(Inputpost.Title, 4));
                    post.Title = Inputpost.Title;
                    post.Text = Inputpost.Text;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(Inputpost.Slug))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(Inputpost);
        }



        // GET: post/slug/delete
        /*
         * Returns the delete view, checks if the slug exists in any of posts, then checks for authorization - only the author of the post can access delete page of it
         * The view asks the user for confirmation
         */
        [Authorize]
        [Route("post/{slug}/delete")]
        public async Task<IActionResult> Delete(string slug)
        {
            if (slug.Equals(null))
            {
                return NotFound();
            }
            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Slug.Equals(slug));
            if (post == null)
            {
                return NotFound();
            }

            if (!(await GetCurrentUser()).Id.Equals(post.User_id))
            {
                return StatusCode(403);
            }

            return View(post);
        }



        // POST: Posts/Delete/5
        /*
        * Deletes the post, checks if the slug exists in any of posts, then checks for authorization - only the author of the delete it
        */
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("post/{slug}/delete")]
        public async Task<IActionResult> DeleteConfirmed(string slug)
        {
            var post = await _context.Post
               .FirstOrDefaultAsync(m => m.Slug.Equals(slug));

            //Authorize
            if (!(await GetCurrentUser()).Id.Equals(post.User_id))
            {
                return StatusCode(403);
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        private bool PostExists(string slug)
        {
            return _context.Post.Any(e => e.Slug.Equals(slug));
        }
    }
}
