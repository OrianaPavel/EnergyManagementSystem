using Microsoft.AspNetCore.Mvc;
using DeviceService.Entities;
using DeviceService.Dtos;
using System;
using DeviceService.Repositories;
using HashidsNet;
using Microsoft.AspNetCore.Authorization;

namespace DeviceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        
        private readonly ILogger<UserController> _logger;
        private readonly IHashids _hashids;
        public UserController(IUserRepo userRepo, IHashids hashids, ILogger<UserController> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST: api/User
        [HttpPost]
        public ActionResult<User> CreateUser(string userId)
        {
            int rawId = getRawId(userId);
            if(rawId == -1)
            {
                return NotFound();
            }
            User? createdUser = _userRepo.CreateUser(new User { UserId = rawId });
            if(createdUser == null){
                return BadRequest();
            }
            return CreatedAtAction(nameof(CreateUser), createdUser); 
        }

        // DELETE: api/User/{userId}
        [HttpDelete("{userId}")]
        public ActionResult DeleteUser(string userId)
        {
            int rawId = getRawId(userId);
            if(rawId == -1)
            {
                return NotFound();
            }
            
            _userRepo.DeleteUser(rawId);
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
