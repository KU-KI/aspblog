using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net;

namespace AspBlog.Controllers
{
    [Route("/errors")]
    public class ErrorsController : Controller
    {

        /*
         * Depending on error code, results in a error view page with an error message (default or 403 or 404) 
         * 
         * @param int code 
         */
        [Route("{code}")]
        public IActionResult Error(int code)
        {
            Response.StatusCode = code;
            ViewData["Code"] = code;
            ViewData["Heading"] = " Chyba";
            ViewData["Message"] = "Ups! Niekde nastala chyba.";
            switch (code)
            {
                case 404:
                    {
                        ViewData["Heading"] = "Stránka nebola nájdená.";
                        ViewData["Message"] = "Na danej adrese sme nič nenašli.";
                        break;
                    }

                case 403:
                    {
                        ViewData["Heading"] = "Prístup zamietnutý.";
                        ViewData["Message"] = "Na vykonanie tejto akcie nemáte dostatočné práva.";
                        break;
                    }
            }
            return View("Error");
        }
    }
}