using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public AdminService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }
        public async Task CreateAdmin(AdminRequestDTO model)
        {
            var admin = _mapper.Map<Admin>(model);

            await _databaseContext.Admins.AddAsync(admin);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Admin?>> GetAll()
        {
            var fromDb = await _databaseContext.Admins.ToListAsync();
            return fromDb;
        }

        public async Task<Admin> GetById(int id)
        {
            var result = await _databaseContext.Admins.Where(l => l.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("Admin not found");
            }

            return result;
        }

        public async Task RemoveAdmin(Admin admin)
        {
            _databaseContext.Admins.Remove(admin);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAdmin(Admin admin)
        {
            _databaseContext.Update(admin);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
