using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TODOList.Entities
    {
    public class User
        {
        public int Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "E-mail is not valid.")]
        public string Email { get; set; }
        [Required]
        [StringLength (50, ErrorMessage = "Password must be at least 12 symbols long.", MinimumLength = 12)]
        [MinLength(12)]
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        public virtual ICollection<TodoItem> Todos { get; set; }
        }
    }
