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
    [Route("[controller]")]
    //[Authorize]
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

        // Post: api/User/Id
        [HttpPost("{id}")]
        [Authorize]
        [ValidateIdClaim]
        public ActionResult<User> CreateUser(string id)
        {
            int rawId = getRawId(id);
            _logger.LogInformation($"DeviceServe createUser userId received = {id} and rawId = {rawId} <-------");
            if(rawId == -1)
            {
                return NotFound();
            }

            User createdUser = _userRepo.CreateUser(new User { Id = rawId });
            if(createdUser == null){
                return BadRequest();
            }
            return CreatedAtAction(nameof(CreateUser), createdUser); 
        }

        // DELETE: api/User/{userId}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
