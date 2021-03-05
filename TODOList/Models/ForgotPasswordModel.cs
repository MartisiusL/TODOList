using System.ComponentModel.DataAnnotations;

namespace TODOList.Models
    {
    public class ForgotPasswordModel
        {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        }
    }
