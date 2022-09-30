
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
            ErrorMessage="Password must have 1 Uppercase,1 lowercase,1 number and 1 nonalpha and minimum of 6 characters")]
        public string Password { get; set; }
    }
}
