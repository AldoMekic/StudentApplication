using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task CreateEnrollment(EnrollmentRequestDTO model)
        {
            var enrollment = _mapper.Map<Enrollment>(model);

            await _databaseContext.Enrollments.AddAsync(enrollment);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Enrollment?>> GetAll()
        {
            var fromDb = await _databaseContext.Enrollments.ToListAsync();
            return fromDb;
        }

        public async Task<Enrollment> GetById(int id)
        {
            var result = await _databaseContext.Enrollments.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Enrollment not found");
            }

            return result;
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
