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
        public async Task<IActionResult> Get()
        {
            string sql = @"SELECT o.Id AS OrderId,
                                        o.PaymentTypeId,
                                        o.CustomerId AS CusId,
                                        c.FirstName,
                                        c.LastName,
                                        pt.AcctNumber,
                                        pt.Name,
                                        pt.CustomerId AS PayCusId,
                                        p.Id AS ProductId,
                                        p.Price,
                                        p.Title,
                                        p.Description,
                                        p.Quantity,
                                        p.ProductTypeId,
                                        prodt.Id AS ProductTypeId,
                                        prodt.Name
                                        FROM Order o
                                        LEFT JOIN Customer c ON c.Id = o.CustomerId
                                        LEFT JOIN PaymentType pt ON pt.Id = o.PaymentTypeId
                                        LEFT JOIN OrderProduct op ON op.OrderId = o.Id
                                        LEFT JOIN Product p ON p.Id = op.ProductId
                                        LEFT JOIN ProductType prodt ON prodt.Id = p.ProductTypeId
                                        WHERE 2=2";
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Order> orderHash = new Dictionary<int, Order>();

                    List<Order> orders = new List<Order>();
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
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CusId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                },
                                PaymentType = new PaymentType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("PayCusId"))
                                }
                            };
                        }

                        orderHash[orderId].Products.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title

                            Description
                            Quantity
                            CustomerId
                            ProductTypeId
                            ProductType = new ProductType
                            {
                                Id
                                Name
                            }
                        });

                        orders.Add(order);
                    }
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
