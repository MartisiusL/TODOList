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

        /// <summary>
        /// Password reset procedure forgot password endpoint.
        /// </summary>
        /// <param name="forgotPasswordModel">Model of forgot password with forgotten account email.</param>
        /// <returns>Ok result and sends password recovery email.</returns>
        /// <response code="200">Successfully sent an email or email not found.</response>
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

        /// <summary>
        /// Password reset procedure reset password endpoint.
        /// </summary>
        /// <param name="token">Token for password change procedure, which was sent to the email.</param>
        /// <param name="email">Email of the user to update password.</param>
        /// <param name="resetPasswordModel">Model of reset password, which has new password in it.</param>
        /// <returns>Ok result and updates password.</returns>
        /// <response code="200">Successfully changed password or email not found.</response>
        /// <response code="400">Bad request with an error.</response>
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
