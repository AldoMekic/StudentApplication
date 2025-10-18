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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _adminService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Admin?>, IEnumerable<AdminResponseDTO>>(admins));
        }

        [HttpGet("{id:int}", Name = nameof(GetAdmin))]
        public async Task<IActionResult> GetAdmin(int id)
        {
            var admin = await _adminService.GetById(id);
            return Ok(_mapper.Map<Admin, AdminResponseDTO>(admin));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminRequestDTO admin)
        {
            await _adminService.CreateAdmin(admin);
            var created = (await _adminService.GetAll()).First(a => a!.Username == admin.Username)!;
            return CreatedAtAction(nameof(GetAdmin), new { id = created.Id }, _mapper.Map<Admin, AdminResponseDTO>(created));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminService.GetById(id);
            await _adminService.RemoveAdmin(admin);
            return NoContent();
        }
    }
}
