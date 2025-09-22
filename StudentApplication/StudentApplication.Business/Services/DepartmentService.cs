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
    public class DepartmentService : IDepartmentService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public DepartmentService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task CreateDepartment(DepartmentRequestDTO model)
        {
            var department = _mapper.Map<Department>(model);

            await _databaseContext.Departments.AddAsync(department);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Department?>> GetAll()
        {
            var fromDb = await _databaseContext.Departments.ToListAsync();
            return fromDb;
        }

        public async Task<Department> GetById(int id)
        {
            var result = await _databaseContext.Departments.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Department not found");
            }

            return result;
        }

        public async Task RemoveDepartment(Department department)
        {
            _databaseContext.Departments.Remove(department);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateDepartment(Department department)
        {
            _databaseContext.Update(department);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
