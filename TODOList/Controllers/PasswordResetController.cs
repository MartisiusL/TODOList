using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TODOList.Entities;
using TODOList.Models;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route ("[controller]")]
    public class PasswordResetController : ControllerBase
        {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSenderService _emailSender;

        public PasswordResetController (UserManager<User> userManager, IEmailSenderService emailSender)
            {
            _userManager = userManager;
            _emailSender = emailSender;
            }

        [HttpPost ("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword (ForgotPasswordModel forgotPasswordModel)
            {
            var user = await _userManager.FindByEmailAsync (forgotPasswordModel.Email);
            if (user == null)
                return Ok ();
            var token = await _userManager.GeneratePasswordResetTokenAsync (user);
            var callback = Url.Action ("ResetPassword", "PasswordReset", new { token, email = user.Email }, Request.Scheme);
            var message = new Message (new string[] { forgotPasswordModel.Email }, "Reset password token", callback);
            await _emailSender.SendEmailAsync (message);
            return Ok ();
            }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword ([FromQuery] string token, [FromQuery] string email, [FromBody] ResetPasswordModel resetPasswordModel)
            {
            var user = await _userManager.FindByEmailAsync (email);
            if (user == null)
                return Ok ();
            var result = await _userManager.ResetPasswordAsync (user, token, resetPasswordModel.Password);
            if (result.Succeeded)
                return Ok ();
            return BadRequest (result.Errors);
            }
        }
    }
