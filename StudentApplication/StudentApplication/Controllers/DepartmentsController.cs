using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public DepartmentsController(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }

        [HttpGet("getAllDepartments")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<Department?>, IEnumerable<DepartmentResponseDTO>>(await _departmentService.GetAll()));
        }


        [HttpDelete("deleteDepartment/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentService.GetById(id);

            await _departmentService.RemoveDepartment(department);

            return Ok(department);
        }


        [HttpGet("getDepartmentById/{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var department = await _departmentService.GetById(id);

            return Ok(_mapper.Map<Department, DepartmentResponseDTO>(department));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequestDTO department)
        {
            await _departmentService.CreateDepartment(department);

            return Ok(department);
        }
    }
}
