using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IEnrollmentService
    {
        Task CreateEnrollment(EnrollmentRequestDTO model);
        Task<IEnumerable<Enrollment?>> GetAll();
        Task<Enrollment> GetById(int id);
        Task RemoveEnrollment(Enrollment enrollment);
        Task UpdateEnrollment(Enrollment enrollment);
    }
}
