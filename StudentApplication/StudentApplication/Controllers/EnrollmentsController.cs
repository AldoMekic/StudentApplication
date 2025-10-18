using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;

        public EnrollmentsController(IEnrollmentService enrollmentService, IMapper mapper)
        {
            _enrollmentService = enrollmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _enrollmentService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Enrollment?>, IEnumerable<EnrollmentResponseDTO>>(all));
        }

        [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetById(id);
            return Ok(_mapper.Map<Enrollment, EnrollmentResponseDTO>(enrollment));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequestDTO enrollment)
        {
            var created = await _enrollmentService.CreateEnrollment(enrollment);
            var dto = _mapper.Map<Enrollment, EnrollmentResponseDTO>(await _enrollmentService.GetById(created.Id));
            return CreatedAtAction(nameof(GetEnrollment), new { id = created.Id }, dto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetById(id);
            await _enrollmentService.RemoveEnrollment(enrollment);
            return NoContent();
        }
    }
}
