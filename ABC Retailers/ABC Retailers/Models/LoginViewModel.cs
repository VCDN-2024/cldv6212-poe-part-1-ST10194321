using System.ComponentModel.DataAnnotations;

namespace ABC_Retailers.Models
{
    // ViewModel used to represent the data for the login form
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
