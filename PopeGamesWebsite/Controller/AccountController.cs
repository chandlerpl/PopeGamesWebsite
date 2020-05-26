using CPopeWebsite.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CPopeWebsite.Controller
{
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDataProtector _dataProtector;

        public AccountController(IDataProtectionProvider dataProtectionProvider, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("SignIn");
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("Account/ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                ViewBag.Message = "Null code or userid";
                return View("Error");
            }
            token = token.Replace(' ', '+');
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = "Null User";
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            ViewBag.Message = result.Succeeded ? "Confirmed email" : "Failed to confirm";

            if (result.Succeeded)
                return Redirect("/");
            else
                return View("Error");
        }

        [HttpPost("Account/SendConfirmation")]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] IdentityUser user)
        {
            var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var smtpClient = new SmtpClient("mail.cpope.uk")
            {
                Port = 25,
                Credentials = new NetworkCredential("donotreply@cpope.uk", "PCGamerFTW4721!"),
                EnableSsl = false,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("donotreply@cpope.uk"),
                Subject = "Confirm your email for Pope Games",
                Body = "<h1>Hello, " + user.UserName + "</h1>" +
            "<p>To complete the signup, please confirm your email address by clicking this link:</p>" +
            "<a href=\"https://www.cpope.uk/Account/Verification?userid=" + user.Id + "&token=" + confirmationCode + "\">https://www.cpope.uk/Account/ConfirmEmail?userid=" + user.Id + "&token=" + confirmationCode + "</a>" +
            
            "<p>Welcome to Pope Games</p>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(user.Email);

            smtpClient.Send(mailMessage);

            return Ok();
        }

        [Authorize]
        [HttpGet("Account/Logout")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return Redirect("/");
        }
    }
}
