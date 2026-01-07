using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentApplication.Controllers
{
    [Route("api/grades")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GradesController(IGradeService gradeService, IUserService userService, IMapper mapper)
        {
            _gradeService = gradeService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var grades = await _gradeService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Grade?>, IEnumerable<GradeResponseDTO>>(grades));
        }

        [HttpGet("{id:int}", Name = nameof(GetGrade))]
        public async Task<IActionResult> GetGrade(int id)
        {
            var grade = await _gradeService.GetById(id);
            return Ok(_mapper.Map<Grade, GradeResponseDTO>(grade));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] GradeRequestDTO grade)
        {
            var created = await _gradeService.CreateGrade(grade);
            return CreatedAtAction(nameof(GetGrade), new { id = created.Id }, _mapper.Map<Grade, GradeResponseDTO>(created));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _gradeService.GetById(id);
            await _gradeService.RemoveGrade(grade);
            return NoContent();
        }

        [HttpPost("{id:int}/request-annulment")]
        public async Task<IActionResult> RequestAnnulment(int id)
        {
            var updated = await _gradeService.RequestAnnulment(id);
            // reload fully for enriched mapping
            var full = await _gradeService.GetById(updated.Id);
            return Ok(_mapper.Map<GradeResponseDTO>(full));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyGrades()
        {
            var username =
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized("No username");

            var user = await _userService.GetByUsername(username);
            if (user == null) return Unauthorized("Can't find user");
            if (!user.IsStudent) return Forbid();

            var grades = await _gradeService.GetGradesForStudentUserId(user.Id);
            return Ok(grades);
        }
    }
}
