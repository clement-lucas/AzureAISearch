using DocumentSearchPortal.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DocumentSearchPortal.Controllers.Home
{
    /// <summary>
    /// Home Controller that controls the landing page
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Landing function
        /// </summary>
        /// <returns>view of landing page</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Privacy page
        /// </summary>
        /// <returns>view of privacy page</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error Page
        /// </summary>
        /// <returns>view of error page</returns>
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View("Error", errorViewModel);
        }
    }
}
