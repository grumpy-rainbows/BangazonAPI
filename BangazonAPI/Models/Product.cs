using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// Author: Brian Jobe, Creates a Product Model with required properties.

namespace BangazonAPI.Models
{
    public class Product
    {
        public int Id { get; set; }che
        [Required]
        public int ProductTypeId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Quantity { get; set; }

        public Customer Customer { get; set; }

        public ProductType ProductType { get; set; }
    }
}
