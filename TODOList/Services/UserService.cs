using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TODOList.Entities;
using TODOList.Helpers;
using TODOList.Models;

namespace TODOList.Services
    {
    public interface IUserService
        {
        Task<User> Authenticate (string email, string password);

        User GetCachedUser ();
        }

    public class UserService : IUserService
        {
        private readonly AppSettings _appSettings;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor m_contextAccessor;

        private readonly string m_cookieHeader = "CachedUser";

        public UserService (IOptions<AppSettings> appSettings, UserManager<User> userManager, IHttpContextAccessor contextAccessor)
            {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            m_contextAccessor = contextAccessor;
            }

        public async Task<User> Authenticate (string email, string password)
            {
            var user = await _userManager.FindByEmailAsync (email);
            if (user == null)
                return null;
            var access = _userManager.PasswordHasher.VerifyHashedPassword (user, user.PasswordHash, password);
            if (access == PasswordVerificationResult.Failed)
                {
                return null;
                }

            // authentication successful so generate jwt token
            user.Token = GenerateSecurityToken (user);

            CacheUser (new CachedUserModel ()
                {
                Id = user.Id,
                });

            return user.WithoutPassword ();
            }

        private void CacheUser (CachedUserModel cachedUserModel)
            {
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddMinutes (60) };
            m_contextAccessor.HttpContext.Response.Cookies.Append (m_cookieHeader, JsonConvert.SerializeObject (cachedUserModel), cookieOptions);
            }

        public User GetCachedUser ()
            {
            var cookie = m_contextAccessor.HttpContext.Request.Cookies[m_cookieHeader];
            if (cookie == null)
                return null;
            var cachedUserModel = JsonConvert.DeserializeObject<CachedUserModel> (cookie);
            if (cachedUserModel == null)
                return null;

            using (var context = new ApplicationDbContext ())
                {
                var user = context.User.FirstOrDefault (u => u.Id == cachedUserModel.Id);
                if (user is null)
                    return null;
                return user;
                }
            }

        private string GenerateSecurityToken (User user)
            {
            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
                {
                Subject = new ClaimsIdentity (new Claim[]
                    {
                    new Claim (ClaimTypes.Name, user.Id.ToString ()),
                    new Claim (ClaimTypes.Role, user.Role)
                    }),
                Expires = DateTime.UtcNow.AddHours (1),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
                };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return tokenHandler.WriteToken (token);
            }
        }
    }
