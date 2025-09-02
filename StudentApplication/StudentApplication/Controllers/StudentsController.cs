using AutoMapper;
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
    }
}
