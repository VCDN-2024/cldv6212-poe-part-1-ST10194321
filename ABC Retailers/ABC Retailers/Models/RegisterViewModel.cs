using System.ComponentModel.DataAnnotations;

namespace ABC_Retailers.Models
{
    // ViewModel used to represent the data for the register form
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

