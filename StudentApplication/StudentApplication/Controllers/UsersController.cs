using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsersController(IUserService userService, IMapper mapper, IConfiguration config)
        {
            _userService = userService;
            _mapper = mapper;
            _config = config;
        }

        //[Authorize]
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<User?>, IEnumerable<UserResponseDTO>>(await _userService.GetAll()));
        }

        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetById(id);

            await _userService.RemoveUser(user);

            return Ok(user.Id);
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetById(id);

            return Ok(_mapper.Map<User, UserResponseDTO>(user));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequestDTO request)
        {
            var user = await _userService.GetByUsername(request.Username);

            if (user != null)
            {
                return BadRequest($"The username {request.Username} is already taken");
            }

            await _userService.CreateUser(request);
            var newUser = await _userService.GetByUsername(request.Username);


            return Ok(newUser);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserRequestDTO login)
        {
            var user = await _userService.GetByUsername(login.Username);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }
        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(null, null, claims, expires: DateTime.UtcNow.AddMinutes(120), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
