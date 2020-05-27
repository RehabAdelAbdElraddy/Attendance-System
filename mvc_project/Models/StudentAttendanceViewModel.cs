using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mvc_project.Models;

namespace mvc_project.ViewModels
{
    public class StudentAttendanceViewModel
    {

        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<IGrouping<string, mvc_project.Models.Attendance>> Attendances { get; set; }
        public IEnumerable<IGrouping<string, mvc_project.Models.Permission>> Permission { get; set; }
        public ApplicationUser Student { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }


    }
}