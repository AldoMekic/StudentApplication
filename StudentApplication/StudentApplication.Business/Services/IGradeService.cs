using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IGradeService
    {
        Task CreateGrade(GradeRequestDTO model);
        Task<IEnumerable<Grade?>> GetAll();
        Task<Grade> GetById(int id);
        Task RemoveGrade(Grade grade);
        Task UpdateGrade(Grade grade);
    }
}
