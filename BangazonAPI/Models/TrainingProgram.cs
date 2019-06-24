/*
    Author : Jameka Echols
    Purpose: The purpose of this file is to create a custom type that will be used to model the data that we 
             have in our database. This particular file is to minic the Training Program table which has the following 
             properties such as: Id, Name, StartDate of the Training program, EndDate of the training program, the 
             MaxAttendees which will hold the capacity limit of each program and there is a list of Employees that are 
             in that particular training program
    Methods: NONE
                                    
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class TrainingProgram<T>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int MaxAttendees { get; set; }

        List<T> EmployeesInTrainingProgram { get; set; } = new List<T>();
    }
}
