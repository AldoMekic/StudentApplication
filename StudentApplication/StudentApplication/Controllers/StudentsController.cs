using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet("getAllStudents")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Student?>, IEnumerable<StudentResponseDTO>>(await _studentService.GetAll()));
        }


        [HttpDelete("deleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentService.GetById(id);

            await _studentService.RemoveStudent(student);

            return Ok(student);
        }


        [HttpGet("getStudentById/{id}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _studentService.GetById(id);

            return Ok(_mapper.Map<Student, StudentResponseDTO>(student));
        }

        [HttpGet("getStudentByName/{name}")]
        public async Task<IActionResult> GetStudentByName(string name)
        {
            var student = await _studentService.GetByName(name);

            return Ok(_mapper.Map<Student, StudentResponseDTO>(student));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentRequestDTO student)
        {
            await _studentService.CreateStudent(student);

            return Ok(student);
        }

        [HttpPost("addSubjectToStudent/{studentId}/{subjectId}")]
        public async Task<IActionResult> AddSubjectToStudent(int studentId, int subjectId)
        {
            await _studentService.AddSubjectToStudent(studentId, subjectId);
            var student = await _studentService.GetById(studentId);

            return Ok(_mapper.Map<Student, StudentResponseDTO>(student));
        }

        [HttpGet("getStudentSubjects/{studentId}")]
        public async Task<IActionResult> GetAllSubjects(int studentId)
        {
            var subjects = await _studentService.GetStudentSubjects(studentId);
            var dto = _mapper.Map<List<SubjectResponseDTO>>(subjects);

            return Ok(dto);
        }

        [HttpDelete("removeStudentSubject/{studentId}/{subjectId}")]
        public async Task<IActionResult> RemoveStudentSubject(int studentId, int subjectId)
        {
            await _studentService.RemoveStudentSubject(studentId, subjectId);

            return Ok(_mapper.Map<Student, StudentResponseDTO>(await _studentService.GetById(studentId)));
        }
    }
}
