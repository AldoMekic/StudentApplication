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

        [HttpGet("getAllEnrollments")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Enrollment?>, IEnumerable<EnrollmentResponseDTO>>(await _enrollmentService.GetAll()));
        }


        [HttpDelete("deleteEnrollment/{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetById(id);

            await _enrollmentService.RemoveEnrollment(enrollment);

            return Ok(enrollment);
        }


        [HttpGet("getEnrollmentById/{id}")]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetById(id);

            return Ok(_mapper.Map<Enrollment, EnrollmentResponseDTO>(enrollment));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequestDTO enrollment)
        {
            await _enrollmentService.CreateEnrollment(enrollment);

            return Ok(enrollment);
        }
    }
}
