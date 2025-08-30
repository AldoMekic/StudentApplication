using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int OfficialGrade { get; set; }
        public float TotalScore { get; set; }
    }
}
