using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TODOList.Entities;
using TODOList.Helpers;
using TODOList.Models;

namespace TODOList.Services
    {
    public interface IUserService
        {
        User Authenticate (string username, string password);

        User GetCachedUser();
        }

    public class UserService : IUserService
        {
        private readonly AppSettings _appSettings;
        private User _cachedUser;

        public UserService (IOptions<AppSettings> appSettings)
            {
            _appSettings = appSettings.Value;
            }

        public User Authenticate (string username, string password)
            {
            using (var context = new TodosContext ())
                {
                var user = context.User.FirstOrDefault (user => user.Email == username && user.Password == password);
                // return null if user not found
                if (user == null)
                    return null;

                _cachedUser = user;
                }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
                {
                Subject = new ClaimsIdentity (new Claim[]
                    {
                    new Claim (ClaimTypes.Name, _cachedUser.Id.ToString ()),
                    new Claim (ClaimTypes.Role, _cachedUser.Role)
                    }),
                Expires = DateTime.UtcNow.AddDays (7),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
                };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            _cachedUser.Token = tokenHandler.WriteToken (token);

            return _cachedUser.WithoutPassword ();
            }

        public User GetCachedUser ()
            {
            return _cachedUser;
            }
        }
    }
