using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public SubjectsController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        [HttpGet("getAllSubjects")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Subject?>, IEnumerable<SubjectResponseDTO>>(await _subjectService.GetAll()));
        }


        [HttpDelete("deleteSubject/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectService.GetById(id);

            await _subjectService.RemoveSubject(subject);

            return Ok(subject);
        }


        [HttpGet("getSubjectById/{id}")]
        public async Task<IActionResult> GetSubject(int id)
        {
            var subject = await _subjectService.GetById(id);

            return Ok(_mapper.Map<Subject, SubjectResponseDTO>(subject));
        }

        [HttpGet("getSubjectByName/{name}")]
        public async Task<IActionResult> GetSubjectByName(string name)
        {
            var subject = await _subjectService.GetByName(name);

            return Ok(_mapper.Map<Subject, SubjectResponseDTO>(subject));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectRequestDTO subject)
        {
            await _subjectService.CreateSubject(subject);

            return Ok(subject);
        }
    }
}
