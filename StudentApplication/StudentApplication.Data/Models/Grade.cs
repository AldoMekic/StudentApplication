using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; } = default!;
        public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
