/* Author: Brian Jobe
* Purpose: Creating a controller for the Computer class. 
* Methods: GET ALL, GET SINGLE, POST, PUT, and DELETE
*/

using System;
using System.Collections.Generic;
using System.Data;
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
    public class ComputerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get(string q, string _includes)
        {
            string sql = @"SELECT c.Id,
                                    c.PurchaseDate, 
                                    c.DecomissionDate,
                                    c.Make, 
                                    c.Manufacturer,
                                    e.Id AS EmployeeId,
                                    e.FirstName, 
                                    e.LastName, 
                                    e.IsSuperVisor, 
                                    e.DepartmentId
                                    FROM Computer c
                                    JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                                    JOIN Employee e ON ce.EmployeeId = e.Id";


            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {

                        Computer computer;

                        if (!computers.Exists(a => a.Id == reader.GetInt32(reader.GetOrdinal("Id"))))
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                Employee = new Employee()
                                {

                                }
                            };
                            computers.Add(computer);
                        }
                        computer = computers.Find(a => a.Id == reader.GetInt32(reader.GetOrdinal("Id")));



                    }

                    reader.Close();

                    return Ok(computers);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ComputerExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            string sql = @"SELECT c.Id, c.FirstName, 
                                    c.LastName
                                    FROM Computer c
                                    WHERE c.Id = @id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Computer computer = null;
                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            // You might have more columns
                        };
                    }

                    reader.Close();

                    return Ok(computer);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Computer (FirstName, LastName)
                        OUTPUT INSERTED.Id
                        VALUES (@firstName, @lastName)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", computer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", computer.LastName));
                    int newId = (int)await cmd.ExecuteScalarAsync();
                    computer.Id = newId;

                    return CreatedAtRoute("GetComputer", new { id = newId }, computer);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Computer computer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Computer
                            SET FirstName = @firstName,
                            LastName = @lastName
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", computer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", computer.LastName));

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
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";
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
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ComputerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Computer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
