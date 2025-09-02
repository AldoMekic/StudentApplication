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
    public class SubjectService : ISubjectService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public SubjectService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task CreateSubject(SubjectRequestDTO model)
        {
            var subject = _mapper.Map<Subject>(model);

            await _databaseContext.Subjects.AddAsync(subject);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subject?>> GetAll()
        {
            var fromDb = await _databaseContext.Subjects.ToListAsync();
            return fromDb;
        }

        public async Task<Subject> GetById(int id)
        {
            var result = await _databaseContext.Subjects.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Subject not found");
            }

            return result;
        }

        public async Task<Subject> GetByName(string name)
        {
            var result = await _databaseContext.Subjects.Where(a => a.Title == name).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Subject not found");
            }

            return result;
        }

        public async Task<Subject> GetFirst()
        {
            var result = await _databaseContext.Subjects.FirstOrDefaultAsync();
            if (result == null)
                throw new Exception("No subjects in database");
            return result;
        }

        public async Task RemoveSubject(Subject subject)
        {
            _databaseContext.Subjects.Remove(subject);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateSubject(Subject subject)
        {
            _databaseContext.Update(subject);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
