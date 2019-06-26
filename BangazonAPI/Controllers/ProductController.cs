/*
    Author: Jameka Echols
    Purpose: The purpose of this controller is to make all the calls to the database to GET, POST, PUT and DELETE
             data specific to the Product table in the BangazonAPI database. 
    Method: I created the GET method which will return a list of products, a GET that will return a
            single product based on the id that was passed in, a POST method which will create a 
            product and also make a GET call and I also created a DELETE method that will 
            delete a product based off the id that passed to it.
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
            using (SqlConnection conn = Connection)
            {
                // step 2 open the connection 
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // step 3 create and run the query
                    cmd.CommandText = $@"SELECT p.Id,
                                                p.ProductTypeId,
                                                p.CustomerId,
                                                p.Price,
                                                p.Title,
                                                p.Description,
                                                p.Quantity,
                                                pt.Name,
                                                c.Id CustomerId,
                                                c.FirstName,
                                                c.LastName,
                                                pt.Id ProductTypeId,
                                                pt.Name ProductTypeName
                                        FROM Product p
                                        JOIN Customer c ON c.Id = p.CustomerId
                                        JOIN ProductType pt ON pt.Id = p.ProductTypeId";

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
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            },
                            ProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("ProductTypeName"))
                            }
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
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT p.Id,
                                                p.ProductTypeId,
                                                p.CustomerId,
                                                p.Price,
                                                p.Title,
                                                p.Description,
                                                p.Quantity,
                                                pt.Name,
                                                c.Id CustomerId,
                                                c.FirstName,
                                                c.LastName,
                                                pt.Id ProductTypeId,
                                                pt.Name ProductTypeName
                                        FROM Product p
                                        JOIN Customer c ON c.Id = p.CustomerId
                                        JOIN ProductType pt ON pt.Id = p.ProductTypeId
                                        WHERE p.Id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Product product = null;
                    while (reader.Read())
                    {
                        if (product == null)
                        {
                            product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                },
                                ProductType = new ProductType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    Name = reader.GetString(reader.GetOrdinal("ProductTypeName"))
                                }
                            };
                        }
                    }
                    // step 5 close the connection and return the single product
                    reader.Close();
                    return Ok(product);
                }
            }
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                // open the connection 
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // create the query and run it 
                    cmd.CommandText = $@"INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity)
                                         OUTPUT INSERTED.id
                                         VALUES (@productTypeId, @customerId, @price, @title, @description, @quantity);";
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));

                    product.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
                }
            }
        }

        // PUT: api/Product/5

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    // open the connection
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Product
                                            SET ProductTypeId = @productTypeId,
                                                CustomerId = @customerId,
                                                Price = @price,
                                                Title = @title,
                                                Description = @description,
                                                Quantity = @quantity
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@productTypeId", product.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@customerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@description", product.Description));
                        cmd.Parameters.Add(new SqlParameter("@quantity", product.Quantity));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    // open the connection 
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        // create the query and run it 
                        cmd.CommandText = $@"DELETE FROM Product 
                                                   WHERE Id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id FROM Product WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
