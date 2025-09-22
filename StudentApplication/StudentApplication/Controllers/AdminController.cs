using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }

        [HttpGet("getAllAdmins")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Admin?>, IEnumerable<AdminResponseDTO>>(await _adminService.GetAll()));
        }


        [HttpDelete("deleteAdmin/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminService.GetById(id);

            await _adminService.RemoveAdmin(admin);

            return Ok(admin);
        }


        [HttpGet("getAdminById/{id}")]
        public async Task<IActionResult> GetAdmin(int id)
        {
            var admin = await _adminService.GetById(id);

            return Ok(_mapper.Map<Admin, AdminResponseDTO>(admin));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminRequestDTO admin)
        {
            await _adminService.CreateAdmin(admin);

            return Ok(admin);
        }
    }
}
