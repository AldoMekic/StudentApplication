using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IProfessorService
    {
        Task CreateProfessor(ProfessorRequestDTO model);
        Task<IEnumerable<Professor?>> GetAll();
        Task<Professor> GetById(int id);
        Task<Professor> GetByName(string name);
        Task<Professor> GetFirst();
        Task RemoveProfessor(Professor professor);
        Task UpdateProfessor(Professor professor);

        Task AddSubjectToProfessor(int professorId, int subjectId);
        Task<List<Subject>> GetProfessorSubjects(int professorId);
        Task RemoveProfessorSubject(int professorId, int subjectId);
    }
}
