using Microsoft.AspNetCore.Mvc;
using DeviceService.Dtos;
using UserDevice.Dtos;
using HashidsNet;
using Microsoft.AspNetCore.Authorization;

namespace DeviceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly Service.DeviceService _deviceService;
        private readonly ILogger<DeviceController> _logger;
        
        private readonly IHashids _hashids;

        public DeviceController(Service.DeviceService deviceService, ILogger<DeviceController> logger, IHashids hashids)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        //TODO
        public ActionResult<DeviceReadDto> CreateDevice(DeviceCreateDto deviceCreateDto)
        {
            if (deviceCreateDto == null)
            {
                
                _logger.LogError("Error creating device.");     
                return BadRequest("User data is null.");
            }

            var deviceReadDto = _deviceService.CreateDevice(deviceCreateDto);
            return CreatedAtAction(nameof(GetDeviceById), new { id = _hashids.Encode(deviceReadDto.Id) }, deviceReadDto);
        }

        [HttpGet("{id}")]
        [ValidateIdClaim]
        public ActionResult<DeviceReadDto> GetDeviceById(string id)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            var device = _deviceService.GetDeviceById(rawId);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(device);
        }
        
        // GET: api/Device/User/{userId}
        [HttpGet("User/{userId}")]
        public ActionResult<IEnumerable<DeviceReadDto>> GetDevicesByUserId(int userId)
        {
            var devices = _deviceService.GetDevicesByUserId(userId);
            if(devices == null || !devices.Any())
            {
                return NotFound();
            }
            return Ok(devices);
        }
        // GET: api/Device
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<DeviceReadDto>> GetAllDevices()
        {
            var devices = _deviceService.GetAllDevices();
            return Ok(devices);
        }

        // PUT: api/Device/{id}
        [HttpPut("{id}")]
        [ValidateIdClaim]
        public ActionResult UpdateDevice(string id, DeviceCreateDto deviceUpdateDto)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            _deviceService.UpdateDevice(rawId, deviceUpdateDto);
            return NoContent();
        }

        // DELETE: api/Device/{id}
        [HttpDelete("{id}")]
        [ValidateIdClaim]
        public ActionResult DeleteDevice(string id)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            var device = _deviceService.GetDeviceById(rawId);
            if(device == null)
            {
                return NotFound();
            }
            
            _deviceService.DeleteDevice(rawId);
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
