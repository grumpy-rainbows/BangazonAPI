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
        public async Task<IActionResult> Get()
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
                                    LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                                    LEFT JOIN Employee e ON ce.EmployeeId = e.Id";

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
                        List<Employee> employees = new List<Employee>();
                        Computer computer;

                        if (!computers.Exists(a => a.Id == reader.GetInt32(reader.GetOrdinal("Id"))))
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                Employees = employees
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                                computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                            }

                            computers.Add(computer);
                        }
                        computer = computers.Find(a => a.Id == reader.GetInt32(reader.GetOrdinal("Id")));

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                            };
                            employees.Add(employee);
                        }
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
                                    LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                                    LEFT JOIN Employee e ON ce.EmployeeId = e.Id
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

                    List<Employee> employees = new List<Employee>();
                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            Employees = employees
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                            };
                            computer.Employees.Add(employee);
                        }
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
                        INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer)
                        OUTPUT INSERTED.Id
                        VALUES (@purchaseDate, @decomissionDate, @make, @manufacturer)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decomissionDate", computer.DecomissionDate));
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
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
                            SET PurchaseDate = @purchaseDate,
                            DecomissionDate = @decomissionDate,
                            Make = @make,
                            Manufacturer = @manufacturer
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@decomissionDate", computer.DecomissionDate));
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));

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
