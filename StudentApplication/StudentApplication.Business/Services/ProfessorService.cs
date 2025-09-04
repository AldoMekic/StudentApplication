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
    public class ProfessorService : IProfessorService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public ProfessorService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task AddSubjectToProfessor(int professorId, int subjectId)
        {
            var professor = await _databaseContext.Professors.Where(u => u.Id == professorId).FirstOrDefaultAsync();

            var subject = await _databaseContext.Subjects.Where(r => r.Id == subjectId).FirstOrDefaultAsync();

            if (professor == null)
                throw new Exception("Professor not found");
            if (subject == null)
                throw new Exception("Subject not found");

            professor.Subjects.Add(subject);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task CreateProfessor(ProfessorRequestDTO model)
        {
            var professor = _mapper.Map<Professor>(model);

            await _databaseContext.Professors.AddAsync(professor);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Professor?>> GetAll()
        {
            var fromDb = await _databaseContext.Professors.Include(p => p.Subjects).ToListAsync();
            return fromDb;
        }

        public async Task<Professor> GetById(int id)
        {
            var result = await _databaseContext.Professors.Where(l => l.Id == id).Include(p => p.Subjects).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Professor not found");
            }

            return result;
        }

        public async Task<Professor> GetByName(string name)
        {
            var result = await _databaseContext.Professors.Where(a => a.FirstName == name).Include(p => p.Subjects).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Professor not found");
            }

            return result;
        }

        public async Task<Professor> GetFirst()
        {
            var result = await _databaseContext.Professors.Include(p => p.Subjects).FirstOrDefaultAsync();
            if (result == null)
                throw new Exception("No professors in database");
            return result;
        }

        public async Task<List<Subject>> GetProfessorSubjects(int professorId)
        {
            var professor = await _databaseContext.Professors.Where(s => s.Id == professorId).Include(s => s.Subjects).FirstOrDefaultAsync();

            return professor.Subjects;
        }

        public async Task RemoveProfessor(Professor professor)
        {
            _databaseContext.Professors.Remove(professor);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task RemoveProfessorSubject(int professorId, int subjectId)
        {
            var professor = await _databaseContext.Professors.Where(st => st.Id == professorId).Include(st => st.Subjects).FirstOrDefaultAsync();

            var subject = await _databaseContext.Subjects.Where(su => su.Id == subjectId).FirstOrDefaultAsync();

            if (professor == null)
                throw new Exception("Professor not found");
            if (subject == null)
                throw new Exception("Subject not found");

            professor.Subjects.Remove(subject);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateProfessor(Professor professor)
        {
            _databaseContext.Update(professor);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
