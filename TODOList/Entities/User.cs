using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TODOList.Entities
    {
    public class User : IdentityUser
        {
        [Required]
        [StringLength (50, ErrorMessage = "Password must be at least 12 symbols long.", MinimumLength = 12)]
        [MinLength(12)]
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        public virtual ICollection<TodoItem> Todos { get; set; }
        }
    }
