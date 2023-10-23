using HashidsNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Dtos;


namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly Service.UserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IHashids _hashids;
        private readonly HelperCallDeviceService _helperService;

        public UserController(Service.UserService userService, ILogger<UserController> logger, IHashids hashids,HelperCallDeviceService helperService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }
        
        // GET: api/Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<UserReadDto>> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            foreach (var user in users)
            {
                user.Hashid = _hashids.Encode(user.Id);
                user.Id = 0;
            }

            return Ok(users);
        }

        // GET: api/Users/5
        [ValidateIdClaim]
        [HttpGet("{id}")]
        public ActionResult<UserReadDto> GetUserById(string id)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            var user = _userService.GetUserById(rawId);
            if (user == null)
            {
                return NotFound();
            }
            user.Hashid = id;
            user.Id = 0;
            return Ok(user);
        }

        // POST: api/Users
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateUser(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                return BadRequest("User data is null.");
            }
            
            userCreateDto.Password = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);
            var user = _userService.CreateUser(userCreateDto);

            var token = Request.Headers["Authorization"].ToString().Replace("bearer ", "");

            _logger.LogInformation("TOKEN IS ->>>>>>> " + token);

            _helperService.CreateUserInDevice(_hashids.Encode(user.Id), token);

            return CreatedAtAction(nameof(GetUserById), new { Id = _hashids.Encode(user.Id) }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateUser(string id, UserCreateDto userUpdateDto)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            if (userUpdateDto == null)
            {
                return BadRequest("User data is null.");
            }

            var existingUser = _userService.GetUserById(rawId);
            if (existingUser == null)
            {
                return NotFound();
            }
            userUpdateDto.Password = BCrypt.Net.BCrypt.HashPassword(userUpdateDto.Password);
            _userService.UpdateUser(rawId, userUpdateDto);
            return NoContent();
        }

        // DELETE: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(string id)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            var user = _userService.GetUserById(rawId);
            if (user == null)
            {
                return NotFound();
            }

            _userService.DeleteUser(rawId);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _helperService.DeleteUser(id, token);

            return NoContent();
        }

        private int getRawId(string hashid){
            var rawId = _hashids.Decode(hashid);
            if(rawId.Length == 0)
            {
                return -1;
            }
            return rawId[0];
        }
    }
}