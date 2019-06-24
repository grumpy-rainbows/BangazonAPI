using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Computer
    {
        public int id { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public DateTime DecomissionDate { get; set; }

        public string Make { get; set; }

        [Required]
        public string Manufacturer { get; set; }

    }
}
