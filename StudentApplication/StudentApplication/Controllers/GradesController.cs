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

        [HttpGet("getAllGrades")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Grade?>, IEnumerable<GradeResponseDTO>>(await _gradeService.GetAll()));
        }


        [HttpDelete("deleteGrade/{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _gradeService.GetById(id);

            await _gradeService.RemoveGrade(grade);

            return Ok(grade);
        }


        [HttpGet("getGradeById/{id}")]
        public async Task<IActionResult> GetGrade(int id)
        {
            var grade = await _gradeService.GetById(id);

            return Ok(_mapper.Map<Grade, GradeResponseDTO>(grade));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] GradeRequestDTO grade)
        {
            await _gradeService.CreateGrade(grade);

            return Ok(grade);
        }
    }
}
