using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TODOList.Entities
    {
    public class User : IdentityUser
        {
        public string Role { get; set; }
        public string Token { get; set; }

        public virtual ICollection<TodoItem> Todos { get; set; }
        }
    }
