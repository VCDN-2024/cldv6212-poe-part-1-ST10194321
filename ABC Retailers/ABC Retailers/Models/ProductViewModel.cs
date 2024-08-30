using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ABC_Retailers.Models
{
    // ViewModel used for handling product creation and editing
    public class ProductViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; } 
    }
}
