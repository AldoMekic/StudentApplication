using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IUserService
    {
        Task CreateUser(UserRegisterRequestDTO model);
        Task<IEnumerable<User?>> GetAll();
        Task<User> GetById(int id);
        Task<User?> GetByUsername(string name);
        Task RemoveUser(User user);
        Task UpdateUser(User user);
    }
}
