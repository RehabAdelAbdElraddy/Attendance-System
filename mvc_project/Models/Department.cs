using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mvc_project.Models
{
    public class Department
    {
        [Key]
        [Display(Name = "Department")]
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}