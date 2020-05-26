using Microsoft.AspNetCore.Mvc;
using CPope.SimplexNoise;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;

namespace CPopeWebsite.Controller
{
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment environment;

        public ImageController(IWebHostEnvironment environment, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route(Urls.UploadImage)]
        [HttpPost]
        public async Task Post()
        {
            if (HttpContext.Request.Form.Files.Any())
            {
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    var path = Path.Combine(environment.ContentRootPath, "../images");

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    path = Path.Combine(path, file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
        }

        [Route(Urls.StaticImage)]
        [HttpGet]
        public IActionResult GetStaticNoise()
        {
            return File(StaticSimplexNoise.NoiseMap2D(4, 1280, 720), "image/jpeg");
        }
    }
}