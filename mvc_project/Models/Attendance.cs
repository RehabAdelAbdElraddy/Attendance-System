using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_project.Models
{
    public class Attendance
    {
        [Key]
        public int AttId { get; set; }
        [ForeignKey("User")]
        public string StdId { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime AttDate { get; set; }
        public string TimeAttend { get; set; }
        public string TimeLeft { get; set; }

        [NotMapped]
        public bool Attend
        {
            get { return !String.IsNullOrEmpty(TimeAttend) ? true : false; }

            private set { /* needed for EF */ }
        }

        [NotMapped]

        public bool OnTime
        {
            get
            { /*return Attend && (TimeAttend.Replace(" ", String.Empty) == "9") ? true : false;*/

                if (Attend && int.Parse(TimeAttend.Split(':')[0]) <= 9)
                {
                    if (int.Parse(TimeAttend.Split(':')[0]) == 9)
                    {
                        if (int.Parse(TimeAttend.Split(':')[1].Split(' ')[0]) < 5)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                }
                else
                {
                    return false;
                }
            }

            private set { /* needed for EF */ }
        }

        [NotMapped]
        public bool Late
        {
            get { return Attend && !OnTime ? true : false; }

            private set { /* needed for EF */ }
        }
    }
}