using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.Security.Claims;

namespace StudentApplication.Controllers
{
    //[Authorize(Policy ="AdminOnly")]
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IUserService userService ,IMapper mapper)
        {
            _adminService = adminService;
            _userService = userService;
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

        [HttpGet("unapproved-professors")]
        public async Task<IActionResult> GetUnapprovedProfessors()
        {
            var pending = await _userService.GetUnapprovedProfessors();
            var result = pending.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email
            });
            return Ok(result);
        }

        [HttpPut("approve/{userId:int}")]
        public async Task<IActionResult> ApproveProfessor(int userId)
        {
            var adminIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var adminName = User?.Identity?.Name ?? "admin";
            int.TryParse(adminIdClaim, out var adminId);

            await _userService.ApproveProfessor(userId, adminId, adminName);
            return NoContent();
        }
    }
}
