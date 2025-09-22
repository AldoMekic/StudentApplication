using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string StudentEnrolled { get; set; }
        public string SubjectEnrolled { get; set; }
    }
}
