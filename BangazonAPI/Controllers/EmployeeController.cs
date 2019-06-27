/*
  Author: Ali Abdulle
                Purpose: Creating a Controller Class
                Mothod: Get, Post, and Put
 */

using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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

        //Get all Employee, depertment, nad Cumputer
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                            SELECT
                                                e.Id,
                                                e.FirstName,
                                                e.LastName,
                                                e.IsSuperVisor,
                                                e.DepartmentId,
                                                d.Name,
                                                ComputerId,
                                                c.PurchaseDate As 'Computer Purchase Date',
                                                c.DecomissionDate AS 'Computer Decomission Date',
                                                c.Make As 'Computer Model',
                                                c.Manufacturer As 'Computer Manufacturer'
                                                FROM Employee e
                                                LEFT Join Department d ON e.DepartmentId = d.Id
                                                LEFT Join ComputerEmployee ce ON e.Id = ce.EmployeeID
                                                LEFT Join Computer c ON ce.ComputerId = c.Id";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee newEmployee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            newEmployee.ComputerId = reader.GetInt32(reader.GetOrdinal("ComputerId"));
                            newEmployee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Computer Purchase Date")),
                                Make = reader.GetString(reader.GetOrdinal("Computer Model")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Computer Manufacturer")),
                                // DecomissionDate = reader.IsDBNull(reader.GetOrdinal("ComputerDecomissionDate")) ? (DateTime?)reader.GetDateTime(reader.GetOrdinal("ComputerDecomissionDate"))


                            };
                        }
                        employees.Add(newEmployee);
                    };

                    reader.Close();
                    return employees;
                }
            }
        }


        //Get all Employee and depertmentId
        [HttpGet("{Id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute]int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                            SELECT
                                                e.Id,
                                                e.FirstName,
                                                e.LastName,
                                                e.IsSuperVisor,
                                                e.DepartmentId,
                                                d.Name,
                                                c.Id AS ComputerId,
                                                c.PurchaseDate As ComputerPurchaseDate,
                                                c.DecomissionDate AS ComputerDecomissionDate,
                                                c.Make As ComputerModel,
                                                c.Manufacturer As ComputerManufacturer
                                                FROM Employee e
                                                LEFT Join Department d ON e.DepartmentId = d.Id
                                                LEFT Join ComputerEmployee ce ON e.Id = ce.EmployeeID
                                                LEFT Join Computer c ON ce.ComputerId = c.Id
                                                WHERE e.Id =@Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", Id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    Employee employees = null;

                    while (reader.Read())
                    {
                        Employee newEmployee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            Computer = new Computer
                            {
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("ComputerPurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerModel")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("ComputerManufacturer"))
                            }

                        };
                        employees = newEmployee;

                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerDecomissionDate")))
                        {

                            employees.Computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("ComputerDecomissionDate"));

                        }

                    }
                    reader.Close();
                    return Ok(employees);
                }
            }

        }

        //Post api:Employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee newEmployee)
        {
            using (SqlConnection conn = Connection)
            {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO Employee (FirstName, LastName, IsSuperVisor, DepartmentId) 
                                                                OUTPUT INSERTED.Id 
                                                                    VALUES (@FirstName, @LastName, @IsSuperVisor, @DepartmentId)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", newEmployee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", newEmployee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@IsSuperVisor", newEmployee.IsSuperVisor));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", newEmployee.DepartmentId));

                    int newId = (int)cmd.ExecuteNonQuery();
                    newEmployee.Id = newId;
                    return CreatedAtRoute("GetEmployeeById", new { Id = newId }, newEmployee);
                }
            }
        }

        //Put  Product Type
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO Employee (FirstName, LastName, IsSuperVisor, DepartmentId) 
                                                                OUTPUT INSERTED.Id 
                                                                    VALUES (@FirstName, @LastName, @IsSuperVisor, @DepartmentId)";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@IsSuperVisor", employee.IsSuperVisor));
                        cmd.Parameters.Add(new SqlParameter("@DepartmentId", employee.DepartmentId));

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
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

            private bool EmployeeExists(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id FROM Employee WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();
                        return reader.Read();
                    }

                }


            }
     }
}