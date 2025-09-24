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

        public async Task AddGradeToStudent(int studentId, int gradeId)
        {
            var student = await _databaseContext.Students.Where(u => u.Id == studentId).FirstOrDefaultAsync();

            var grade = await _databaseContext.Grades.Where(r => r.Id == gradeId).FirstOrDefaultAsync();

            if (student == null)
                throw new Exception("Student not found");
            if (grade == null)
                throw new Exception("Grade not found");

            student.Grades.Add(grade);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task AddSubjectToStudent(int studentId, int subjectId)
        {
        //    var student = await _databaseContext.Students.Where(u => u.Id == studentId).FirstOrDefaultAsync();

        //    var subject = await _databaseContext.Subjects.Where(r => r.Id == subjectId).FirstOrDefaultAsync();

        //    if (student == null)
        //        throw new Exception("Student not found");
        //    if (subject == null)
        //        throw new Exception("Subject not found");

        //    student.Subjects.Add(subject);

        //    await _databaseContext.SaveChangesAsync();

        throw new NotImplementedException();
        }

        public async Task CreateStudent(StudentRequestDTO model)
        {
            var student = _mapper.Map<Student>(model);

            await _databaseContext.Students.AddAsync(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student?>> GetAll()
        {
            var fromDb = await _databaseContext.Students.Include(u => u.Grades).ToListAsync();
            return fromDb;
        }

        public async Task<Student> GetById(int id)
        {
            var result = await _databaseContext.Students.Where(l => l.Id == id).Include(u => u.Grades).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Student not found");
            }

            return result;
        }

        public async Task<Student> GetByName(string name)
        {
            var result = await _databaseContext.Students.Where(a => a.FirstName == name).Include(u => u.Grades).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Student not found");
            }

            return result;
        }

        public async Task<Student> GetFirst()
        {
            var result = await _databaseContext.Students.Include(u => u.Grades).FirstOrDefaultAsync();
            if (result == null)
                throw new Exception("No students in database");
            return result;
        }

        public async Task<List<Grade>> GetStudentGrades(int studentId)
        {
            var student = await _databaseContext.Students.Where(s => s.Id == studentId).Include(s => s.Grades).FirstOrDefaultAsync();

            return student.Grades;
        }

        public async Task<List<Subject>> GetStudentSubjects(int studentId)
        {
        //    var student = await _databaseContext.Students.Where(s => s.Id == studentId).FirstOrDefaultAsync();

        //    return student.Subjects;

        throw new NotImplementedException();
        }

        public async Task RemoveStudent(Student student)
        {
            _databaseContext.Students.Remove(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task RemoveStudentGrade(int studentId, int gradeId)
        {
            var student = await _databaseContext.Students.Where(st => st.Id == studentId).Include(st => st.Grades).FirstOrDefaultAsync();

            var grade = await _databaseContext.Grades.Where(su => su.Id == gradeId).FirstOrDefaultAsync();

            if (student == null)
                throw new Exception("Student not found");
            if (grade == null)
                throw new Exception("Grade not found");

            student.Grades.Remove(grade);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task RemoveStudentSubject(int studentId, int subjectId)
        {
            //var student = await _databaseContext.Students.Where(st => st.Id == studentId).Include(st => st.Subjects).FirstOrDefaultAsync();

            //var subject = await _databaseContext.Subjects.Where(su => su.Id == subjectId).FirstOrDefaultAsync();

            //if (student == null)
            //    throw new Exception("Student not found");
            //if (subject == null)
            //    throw new Exception("Subject not found");

            //student.Subjects.Remove(subject);

            //await _databaseContext.SaveChangesAsync();

            throw new NotImplementedException();
        }

        public async Task UpdateStudent(Student student)
        {
            _databaseContext.Update(student);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
