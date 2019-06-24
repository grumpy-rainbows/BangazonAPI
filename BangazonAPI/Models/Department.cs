/*
    Author : Jameka Echols
    Purpose: The purpose of this file is to create a custom type that will be used to model the data that we 
             have in our database. This particular file is to minic the Training Program table which has the following 
             properties such as: Id, Name, Budget and a list of employees in that particular department
    Methods: NONE
                                    
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }
        public List<Employee> DepartmentEmployees { get; set; } = new List<Employee>();
    }
}
