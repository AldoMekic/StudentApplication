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
        public async Task CreateProfessor(ProfessorRequestDTO model)
        {
            var professor = _mapper.Map<Professor>(model);

            await _databaseContext.Professors.AddAsync(professor);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Professor?>> GetAll()
        {
            var fromDb = await _databaseContext.Professors.ToListAsync();
            return fromDb;
        }

        public async Task<Professor> GetById(int id)
        {
            var result = await _databaseContext.Professors.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Professor not found");
            }

            return result;
        }

        public async Task<Professor> GetByName(string name)
        {
            var result = await _databaseContext.Professors.Where(a => a.FirstName == name).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Professor not found");
            }

            return result;
        }

        public async Task<Professor> GetFirst()
        {
            var result = await _databaseContext.Professors.FirstOrDefaultAsync();
            if (result == null)
                throw new Exception("No professors in database");
            return result;
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
