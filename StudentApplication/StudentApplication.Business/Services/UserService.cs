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

            // Explicitly construct the User to control approval flags
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = Hash(model.Password), // NOTE: demo hashing; consider BCrypt/Argon2 in real apps
                IsStudent = model.IsStudent,
                IsProfessor = model.IsProfessor,

                // Approval rules:
                // - Students: approved by default
                // - Professors: require admin approval (IsApproved = false)
                // - If a user is both (unlikely), treat student rule as "true"
                IsApproved = model.IsStudent ? true : !model.IsProfessor,
                IsAdmin = false
            };

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


        public async Task<IReadOnlyList<User>> GetUnapprovedProfessors()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsProfessor && !u.IsApproved)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task ApproveProfessor(int userId, int approvedByAdminId, string approvedByAdminName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new KeyNotFoundException("User not found");
            if (!user.IsProfessor) throw new InvalidOperationException("User is not a professor");

            user.IsApproved = true;

             user.ApprovedAt = DateTime.UtcNow;
             user.ApprovedByAdminName = approvedByAdminName;

            await _context.SaveChangesAsync();
        }
    }
}