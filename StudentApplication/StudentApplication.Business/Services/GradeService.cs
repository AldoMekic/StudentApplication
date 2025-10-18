using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class GradeService : IGradeService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public GradeService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task<Grade> CreateGrade(GradeRequestDTO model)
        {
            // Ensure enrollment exists
            var enrollment = await _databaseContext.Enrollments
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(e => e.Id == model.EnrollmentId);

            if (enrollment == null) throw new KeyNotFoundException("Enrollment not found");
            if (enrollment.Grade != null) throw new InvalidOperationException("This enrollment already has a grade.");

            var grade = _mapper.Map<Grade>(model);
            await _databaseContext.Grades.AddAsync(grade);
            await _databaseContext.SaveChangesAsync();

            // Optional: keep Student.Grades in sync (read model)
            var student = await _databaseContext.Students.FirstOrDefaultAsync(s => s.Id == enrollment.StudentId);
            if (student != null)
            {
                student.Grades.Add(grade);
                await _databaseContext.SaveChangesAsync();
            }

            return grade;
        }

        public async Task<IEnumerable<Grade?>> GetAll()
        {
            return await _databaseContext.Grades.AsNoTracking().ToListAsync();
        }

        public async Task<Grade> GetById(int id)
        {
            var result = await _databaseContext.Grades.FirstOrDefaultAsync(l => l.Id == id);
            if (result == null) throw new KeyNotFoundException("Grade not found");
            return result;
        }

        public async Task RemoveGrade(Grade grade)
        {
            _databaseContext.Grades.Remove(grade);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateGrade(Grade grade)
        {
            _databaseContext.Update(grade);
            await _databaseContext.SaveChangesAsync();
        }
    }
}