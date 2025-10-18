using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

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
            var subject = await _databaseContext.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null) throw new KeyNotFoundException("Subject not found");

            var professor = await _databaseContext.Professors.FirstOrDefaultAsync(p => p.Id == professorId);
            if (professor == null) throw new KeyNotFoundException("Professor not found");

            subject.ProfessorId = professorId; // authoritative FK update
            await _databaseContext.SaveChangesAsync();
        }

        public async Task ReassignProfessorSubject(int subjectId, int newProfessorId)
        {
            var subject = await _databaseContext.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null) throw new KeyNotFoundException("Subject not found");

            var newProf = await _databaseContext.Professors.FirstOrDefaultAsync(p => p.Id == newProfessorId);
            if (newProf == null) throw new KeyNotFoundException("Professor not found");

            subject.ProfessorId = newProfessorId;
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
            return await _databaseContext.Professors
                .AsNoTracking()
                .Include(p => p.Subjects)
                .Include(p => p.Department)
                .ToListAsync();
        }

        public async Task<Professor> GetById(int id)
        {
            var result = await _databaseContext.Professors
                .Include(p => p.Subjects)
                .Include(p => p.Department)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (result == null) throw new KeyNotFoundException("Professor not found");
            return result;
        }

        public async Task<Professor> GetByName(string name)
        {
            var result = await _databaseContext.Professors
                .Include(p => p.Subjects)
                .Include(p => p.Department)
                .FirstOrDefaultAsync(a => a.FirstName == name);

            if (result == null) throw new KeyNotFoundException("Professor not found");
            return result;
        }

        public async Task<Professor> GetFirst()
        {
            var result = await _databaseContext.Professors
                .Include(p => p.Subjects)
                .Include(p => p.Department)
                .FirstOrDefaultAsync();
            if (result == null) throw new InvalidOperationException("No professors in database");
            return result;
        }

        public async Task<List<Subject>> GetProfessorSubjects(int professorId)
        {
            var professor = await _databaseContext.Professors
                .Include(s => s.Subjects)
                .FirstOrDefaultAsync(s => s.Id == professorId);

            if (professor == null) throw new KeyNotFoundException("Professor not found");
            return professor.Subjects;
        }

        public async Task RemoveProfessor(Professor professor)
        {
            _databaseContext.Professors.Remove(professor);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateProfessor(Professor professor)
        {
            _databaseContext.Update(professor);
            await _databaseContext.SaveChangesAsync();
        }
    }
}