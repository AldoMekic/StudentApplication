using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public StudentService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task CreateStudent(StudentRequestDTO model)
        {
            var student = _mapper.Map<Student>(model);

            await _databaseContext.Students.AddAsync(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student?>> GetAll()
        {
            var fromDb = await _databaseContext.Students.ToListAsync();
            return fromDb;
        }

        public async Task<Student> GetById(int id)
        {
            var result = await _databaseContext.Students.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Student not found");
            }

            return result;
        }

        public async Task<Student> GetByName(string name)
        {
            var result = await _databaseContext.Students.Where(a => a.FirstName == name).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Student not found");
            }

            return result;
        }

        public async Task<Student> GetFirst()
        {
            var result = await _databaseContext.Students.FirstOrDefaultAsync();
            if (result == null)
                throw new Exception("No students in database");
            return result;
        }

        public async Task RemoveStudent(Student student)
        {
            _databaseContext.Students.Remove(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateStudent(Student student)
        {
            _databaseContext.Update(student);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
