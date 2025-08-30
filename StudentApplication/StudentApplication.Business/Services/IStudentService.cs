using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IStudentService
    {
        Task CreateStudent(StudentRequestDTO model);
        Task<IEnumerable<Student?>> GetAll();
        Task<Student> GetById(int id);
        Task<Student> GetByName(string name);
        Task<Student> GetFirst();
        Task RemoveStudent(Student student);
        Task UpdateStudent(Student student);
    }
}
