using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class GradeResponseDTO
    {
        public int Id { get; set; }
        public int OfficialGrade { get; set; }
        public float TotalScore { get; set; }
        public int StudentId { get; set; }
    }
}
