/*
    Author: Jameka Echols
    Purpose: The purpose of this controller is to make all the calls to the database to GET, POST, PUT and DELETE
             data specific to the Product table in the BangazonAPI database. 
    Method: 
*/


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                // step 1 create a connection to the database 
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        // GET: api/Product
        [HttpGet]

        public async Task<IActionResult> GetAllProducts()
        {
            using(SqlConnection conn = Connection)
            {
                // step 2 open the connection 
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    // step 3 create and run the query
                    cmd.CommandText = @"SELECT Id,
                                                ProductTypeId,
                                                CustomerId,
                                                Price,
                                                Title,
                                                Description,
                                                Quantity
                                        FROM Product;";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    // create a list to hold all the products
                    List<Product> products = new List<Product>();
                                   
                    while (reader.Read())
                    {

                        // step 4. take the results from the query and store them 
                        // create a new product object
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        };

                        // add that new object to the list of products 
                        products.Add(product);
                    }

                    // close the connestion and return the list of products 
                    reader.Close();
                    return Ok(products);
                }
            }
        }









        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "checking";
        }

        // POST: api/Product
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
