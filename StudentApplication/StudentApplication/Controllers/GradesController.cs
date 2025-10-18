using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/grades")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly IMapper _mapper;

        public GradesController(IGradeService gradeService, IMapper mapper)
        {
            _gradeService = gradeService;
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
    }
}
