using System;
using System.ComponentModel.DataAnnotations;

namespace ABC_Retailers.Models
{
    // Represents a product in the system
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; } 

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string ImageUrl { get; set; } 
    }
}
