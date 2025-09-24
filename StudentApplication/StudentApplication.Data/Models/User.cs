using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsStudent { get; set; }
        public bool IsProfessor { get; set; }

        public Student StudentProfile { get; set; }
        public Professor ProfessorProfile { get; set; }
    }
}
