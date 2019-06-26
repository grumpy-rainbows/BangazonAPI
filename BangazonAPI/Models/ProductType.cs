using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

// Author: Brian Jobe, Creates a ProductType Model with required properties.

namespace BangazonAPI.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
