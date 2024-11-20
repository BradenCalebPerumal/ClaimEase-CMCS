using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskOneDraft.Models;

namespace TaskOneDraft.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; //logger for recording logs

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger; //initializing logger
        }

        public IActionResult Index()
        {
            return View(); //returns the index view
        }

        public IActionResult TermsOfUse()
        {
            return View(); //returns the terms of use view
        }

        public IActionResult ContactUs()
        {
            return View(); //returns the contact us view
        }

        public IActionResult AboutUs()
        {
            return View(); //returns the about us view
        }

        public IActionResult Privacy()
        {
            return View(); //returns the privacy view
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //returns the error view with request id
        }
    }
}
