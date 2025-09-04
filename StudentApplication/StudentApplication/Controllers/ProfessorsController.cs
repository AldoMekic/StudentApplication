using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/professors")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly IMapper _mapper;

        public ProfessorsController(IProfessorService professorService, IMapper mapper)
        {
            _professorService = professorService;
            _mapper = mapper;
        }

        [HttpGet("getAllProfessors")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Professor?>, IEnumerable<ProfessorResponseDTO>>(await _professorService.GetAll()));
        }


        [HttpDelete("deleteProfessor/{id}")]
        public async Task<IActionResult> DeleteProfessor(int id)
        {
            var professor = await _professorService.GetById(id);

            await _professorService.RemoveProfessor(professor);

            return Ok(professor);
        }


        [HttpGet("getProfessorById/{id}")]
        public async Task<IActionResult> GetProfessor(int id)
        {
            var professor = await _professorService.GetById(id);

            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpGet("getProfessorByName/{name}")]
        public async Task<IActionResult> GetProfessorByName(string name)
        {
            var professor = await _professorService.GetByName(name);

            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfessor([FromBody] ProfessorRequestDTO professor)
        {
            await _professorService.CreateProfessor(professor);

            return Ok(professor);
        }

        [HttpPost("addSubjectToProfessor/{professorId}/{subjectId}")]
        public async Task<IActionResult> AddSubjectToProfessor(int professorId, int subjectId)
        {
            await _professorService.AddSubjectToProfessor(professorId, subjectId);
            var professor = await _professorService.GetById(professorId);

            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpGet("getProfessorSubjects/{professorId}")]
        public async Task<IActionResult> GetAllSubjects(int professorId)
        {
            var subjects = await _professorService.GetProfessorSubjects(professorId);
            var dto = _mapper.Map<List<SubjectResponseDTO>>(subjects);

            return Ok(dto);
        }

        [HttpDelete("removeProfessorSubject/{professorId}/{subjectId}")]
        public async Task<IActionResult> RemoveProfessorSubject(int professorId, int subjectId)
        {
            await _professorService.RemoveProfessorSubject(professorId, subjectId);

            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(await _professorService.GetById(professorId)));
        }
    }
}
