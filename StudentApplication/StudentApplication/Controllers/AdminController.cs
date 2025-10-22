using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.Security.Claims;

namespace StudentApplication.Controllers
{
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

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("pending-professors")]
        public async Task<IActionResult> GetPendingProfessors()
        {
            var pending = await _userService.GetUnapprovedProfessors();
            // return a minimal projection (you can make a DTO if you prefer)
            var result = pending.Select(u => new {
                u.Id,
                u.Username,
                u.Email
            });
            return Ok(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("approve-professor/{userId:int}")]
        public async Task<IActionResult> ApproveProfessor(int userId)
        {
            // read admin identity from token (for audit)
            var adminName = User?.FindFirstValue(ClaimTypes.Name) ?? User?.Identity?.Name ?? "admin";
            var adminIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            int adminId = 0;
            int.TryParse(adminIdClaim, out adminId);

            await _userService.ApproveProfessor(userId, adminId, adminName);
            return NoContent();
        }
    }
}
