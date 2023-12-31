using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserService.Dtos;

namespace UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Service.UserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IHashids _hashids;
        private readonly HelperCallDeviceService _helperService;

        public AuthController(Service.UserService userService, ILogger<UserController> logger, IConfiguration configuration, IHashids hashids,HelperCallDeviceService helperService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));

        }

        [HttpPost("register")]
        public ActionResult<UserReadDto> Register(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                return BadRequest("User data is null.");
            }
            userCreateDto.Password = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);
            userCreateDto.UserRole = 0;
            var user = _userService.CreateUser(userCreateDto);
            user.Hashid = _hashids.Encode(user.Id);
            
            _logger.LogInformation("THE user.Hashid is == " + user.Hashid + "The user to generate the token is" + user.ToString());
            _helperService.CreateUserInDevice(user.Hashid, CreateToken(user));
            user.Id = 0;
            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<UserReadDto> Login(UserReadDto userReadDto)
        {
            _logger.LogInformation("Username received = " + userReadDto.Username + " Password received = " + userReadDto.Password);
            var user = _userService.GetUserByUsername(userReadDto.Username);
            //Console.WriteLine(user.Password);
            if(user == null)
            {
                return BadRequest("User data is null.");
            }
            _logger.LogDebug("HashPass --> " + user.Password);
            
            if (user.Username != userReadDto.Username)
            {
                return BadRequest("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userReadDto.Password, user.Password))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);
            user.Hashid = _hashids.Encode(user.Id);
            user.Id = 0;
            //return Ok(token);
            return Ok(new 
            {
                Token = token,
                User = user  
            });
        }

        private string CreateToken(UserReadDto user)
        {
            var hashid = _hashids.Encode(user.Id);
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, hashid),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}