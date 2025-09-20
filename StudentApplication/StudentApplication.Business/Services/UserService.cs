using AutoMapper;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using StudentApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudentApplication.Business.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        public UserService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateUser(UserRequestDTO model)
        {
            var user = _mapper.Map<User>(model);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User?>> GetAll()
        {
            var fromDb = await _context.Users.ToListAsync();
            return fromDb;
        }

        public async Task<User> GetById(int id)
        {
            var result = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new Exception("User not found by ID");
            }

            return result;
        }

        public async Task<User> GetByUsername(string name)
        {
            var result = await _context.Users.Where(u => u.Username == name).FirstOrDefaultAsync();

            return result;
        }

        public async Task RemoveUser(User user)
        {
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
