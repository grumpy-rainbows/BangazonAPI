/* Author: Billy Mathison
 * Purpose: Creating a controller for the Order class. 
 * Methods: GET ALL, GET SINGLE, POST, PUT, and DELETE
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
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> Get(string include, bool completed)
        {
            string sql = @"SELECT o.Id AS OrderId,
                                        o.PaymentTypeId,
                                        o.CustomerId AS CusId,
                                        c.FirstName,
                                        c.LastName,
                                        pt.AcctNumber,
                                        pt.Name AS PaymentTypeName,
                                        pt.CustomerId AS PayCusId,
                                        p.Id AS ProductId,
                                        p.Price,
                                        p.Title,
                                        p.Description,
                                        p.Quantity,
                                        p.ProductTypeId,
                                        prodt.Id AS ProductTypeId,
                                        prodt.Name AS ProductTypeName
                                        FROM Order o
                                        LEFT JOIN Customer c ON c.Id = o.CustomerId
                                        LEFT JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                        LEFT JOIN OrderProduct op ON op.OrderId = o.Id
                                        LEFT JOIN Product p ON p.Id = op.ProductId
                                        LEFT JOIN ProductType prodt ON prodt.Id = p.ProductTypeId
                                        WHERE 2=2";

            if (completed)
            {
                sql = $@"{sql} AND o.PaymentTYpeId IS NOT NULL";
            }

            if (completed == false)
            {
                sql = $@"{sql} AND o.PaymentTYpeId IS NULL";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    if (include != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@include", $"%{include}%"));
                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Order> orderHash = new Dictionary<int, Order>();

                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                        if (!orderHash.ContainsKey(orderId))
                        {
                            orderHash[orderId] = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CusId")),
                                PaymentType = new PaymentType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                    Name = reader.GetString(reader.GetOrdinal("PaymentTypeName")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("PayCusId"))
                                }
                            };
                        }

                        if (@include == "products")
                        {
                            orderHash[orderId].Products.Add(new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                ProductType = new ProductType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    Name = reader.GetString(reader.GetOrdinal("ProductTypeName"))
                                }
                            });
                        }
                        if (@include == "customers")
                        {
                            orderHash[orderId].Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CusId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }
                    }
                    List<Order> orders = orderHash.Values.ToList();
                    reader.Close();

                    return Ok(orders);
                }
            }
        }

        // GET: api/Order/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Order
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Order/5
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
