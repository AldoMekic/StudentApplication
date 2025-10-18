using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;
using System.Security.Cryptography;
using System.Text;

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

        public async Task CreateUser(UserRegisterRequestDTO model)
        {
            // Basic uniqueness checks (mirrors DB unique constraints)
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                throw new InvalidOperationException("Username already in use.");
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                throw new InvalidOperationException("Email already in use.");

            var user = _mapper.Map<User>(model);
            user.Password = Hash(model.Password); // NOTE: for demo only; consider BCrypt/Argon2.

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User?>> GetAll()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            var result = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (result == null) throw new KeyNotFoundException("User not found by ID");
            return result;
        }

        public async Task<User?> GetByUsername(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
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

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // uppercase hex
        }
    }
}