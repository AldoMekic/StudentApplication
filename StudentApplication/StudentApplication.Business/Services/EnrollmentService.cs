using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public EnrollmentService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task<Enrollment> CreateEnrollment(EnrollmentRequestDTO model)
        {
            // Validate existence
            var studentExists = await _databaseContext.Students.AnyAsync(s => s.Id == model.StudentId);
            if (!studentExists) throw new KeyNotFoundException("Student not found");

            var subjectExists = await _databaseContext.Subjects.AnyAsync(s => s.Id == model.SubjectId);
            if (!subjectExists) throw new KeyNotFoundException("Subject not found");

            // Uniqueness per (student,subject)
            var already = await _databaseContext.Enrollments.AnyAsync(e => e.StudentId == model.StudentId && e.SubjectId == model.SubjectId);
            if (already) throw new InvalidOperationException("Student is already enrolled in this subject.");

            var enrollment = _mapper.Map<Enrollment>(model);
            await _databaseContext.Enrollments.AddAsync(enrollment);
            await _databaseContext.SaveChangesAsync();

            return enrollment;
        }

        public async Task<IEnumerable<Enrollment?>> GetAll()
        {
            return await _databaseContext.Enrollments
                .AsNoTracking()
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .ToListAsync();
        }

        public async Task<Enrollment> GetById(int id)
        {
            var result = await _databaseContext.Enrollments
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (result == null) throw new KeyNotFoundException("Enrollment not found");
            return result;
        }

        public async Task<Enrollment?> GetByComposite(int studentId, int subjectId)
        {
            return await _databaseContext.Enrollments
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);
        }

        public async Task RemoveEnrollment(Enrollment enrollment)
        {
            _databaseContext.Enrollments.Remove(enrollment);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateEnrollment(Enrollment enrollment)
        {
            _databaseContext.Update(enrollment);
            await _databaseContext.SaveChangesAsync();
        }
    }
}