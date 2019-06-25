/*
    Author : Ali Abdulle
    purpose: Creating ProductType Controller to navigate the product file
 */
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductTypeController(IConfiguration config)
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

        //Get All the productType
        [HttpGet]
        public async Task<IActionResult> GetAllProductTypes()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                                    Id,
                                                    [Name] 
                                                    From ProductType";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<ProductType> productTypesList = new List<ProductType>();


                    while (reader.Read())
                    {
                        ProductType productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        productTypesList.Add(productType);
                    }

                    reader.Close();
                    return Ok(productTypesList);
                }
            }
        }

        //Get Product Type Id
        [HttpGet("{id}", Name = "GetProductTypeById")]

        public async Task<IActionResult> GetProductTypeById([FromRoute] int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                                    pt.Id,
                                                    pt.[Name] 
                                                    From ProductType pt
                                                    WHERE  pt.Id = @Id ";
                    cmd.Parameters.Add(new SqlParameter("@Id", Id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    ProductType productType = null;

                    while (reader.Read())
                    {
                        productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                  
                    }
                    reader.Close();
                    return Ok(productType);

                }
            }
        }



    }
}