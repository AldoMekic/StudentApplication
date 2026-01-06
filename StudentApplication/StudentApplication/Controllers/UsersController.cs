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
    [Route("api/users")]
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(_mapper.Map<IEnumerable<User>, IEnumerable<UserResponseDTO>>(users!));
        }

        [HttpGet("{id:int}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetById(id);
            return Ok(_mapper.Map<User, UserResponseDTO>(user));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetById(id);
            await _userService.RemoveUser(user);
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDTO request)
        {
            await _userService.CreateUser(request);
            var created = await _userService.GetByUsername(request.Username);

            return CreatedAtAction(nameof(GetUser), new { id = created!.Id }, _mapper.Map<User, UserResponseDTO>(created));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO login, [FromServices] IUserService users)
        {
            var user = await users.GetByUsername(login.Username);
            if (user == null) return Unauthorized(); // user not found

            var incomingHash = Hash(login.Password);
            if (!string.Equals(user.Password, incomingHash, StringComparison.OrdinalIgnoreCase))
                return Unauthorized();

            // NEW: block unapproved professors
            if (user.IsProfessor && !user.IsApproved)
                return Unauthorized(new { message = "Professor account is not approved yet." });

            var tokenString = GenerateJSONWebToken(user);
            return Ok(new { token = tokenString });
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim("is_student", userInfo.IsStudent.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean),
                new Claim("is_professor", userInfo.IsProfessor.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean),

                new Claim("is_admin", userInfo.IsAdmin.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean),
                new Claim("is_approved", userInfo.IsApproved.ToString().ToLowerInvariant(), ClaimValueTypes.Boolean),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddYears(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string Hash(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
