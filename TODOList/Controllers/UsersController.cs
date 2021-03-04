using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TODOList.Models;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize]
        [ApiController]
        [Route ("[controller]")]
        public class UsersController : ControllerBase
            {
            private IUserService _userService;

            public UsersController (IUserService userService)
                {
                _userService = userService;
                }

            [AllowAnonymous]
            [HttpPost ("authenticate")]
            public IActionResult Authenticate ([FromBody] AuthenticateModel model)
                {
                var user = _userService.Authenticate (model.Username, model.Password);

                if (user == null)
                    return BadRequest (new { message = "Email or password is incorrect" });

                return Ok (user);
                }
            }
    }
