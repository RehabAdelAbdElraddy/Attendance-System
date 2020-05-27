using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc_project.Models
{
    public class AdminViewModels
    {
        public int UserID { get; set; }
        public string UserName { set; get; }
        //public List<ApplicationUser> stds = new List<ApplicationUser>();
        public string permissionDate { set; get; }
        public string permissionNote { set; get; }
        public int permissionID { get; set; }
        public string permissionStatus { get; set; }

        public string DeptName { get; set; }
    }
}