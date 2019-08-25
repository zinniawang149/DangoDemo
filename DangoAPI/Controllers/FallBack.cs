using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DangoAPI.Controllers
{
    // Add for SPA:
    // After SPA build in API/wwwroot, we need a route setup to tell which page is the index,
    // or the .net core can't find the route in controller than gives errors.
    public class FallBack : Controller
    {
        public IActionResult Index() {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
    }
}