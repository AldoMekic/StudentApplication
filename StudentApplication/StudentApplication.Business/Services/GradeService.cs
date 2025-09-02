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
    public class GradeService : IGradeService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public GradeService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task CreateGrade(GradeRequestDTO model)
        {
            var grade = _mapper.Map<Grade>(model);

            await _databaseContext.Grades.AddAsync(grade);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Grade?>> GetAll()
        {
            var fromDb = await _databaseContext.Grades.ToListAsync();
            return fromDb;
        }

        public async Task<Grade> GetById(int id)
        {
            var result = await _databaseContext.Grades.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Grade not found");
            }

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
