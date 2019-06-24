/* Author: Billy Mathison
 * Purpose: Creating a class of Order to store information related to the order, including id of the customer and id of the payment type. 
 * Methods: None
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int PaymentTypeId { get; set; }

        public Customer Customer { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
