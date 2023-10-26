using Microsoft.AspNetCore.Mvc;
using DeviceService.Dtos;
using UserDevice.Dtos;
using HashidsNet;
using Microsoft.AspNetCore.Authorization;

namespace DeviceService.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpPost("{id}")]
        [ValidateIdClaim]
        public ActionResult<DeviceReadDto> CreateDevice(DeviceCreateDto deviceCreateDto)
        {
            if (deviceCreateDto == null)
            {
                
                _logger.LogError("Error creating device.");     
                return BadRequest("User data is null.");
            }

            var deviceReadDto = _deviceService.CreateDevice(deviceCreateDto);
            //return CreatedAtAction(nameof(GetDeviceById), new { id = deviceReadDto.Id }, deviceReadDto);
            return Ok(deviceReadDto);
        }

        [HttpGet("{id}/{deviceId}")]
        [ValidateIdClaim]
        public ActionResult<DeviceReadDto> GetDeviceById(string id, string deviceId)
        {
            var rawId = getRawId(deviceId);
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
        
        // GET: api/Device/User/{id}
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<DeviceReadDto>> GetDevicesByUserId(string id)
        {
            var rawId = getRawId(id);
            if(rawId == -1)
            {
                return NotFound();
            }
            var devices = _deviceService.GetDevicesByUserId(rawId);
            /*if(devices == null || !devices.Any())
            {
                return NotFound();
            }*/
            return Ok(devices);
        }
        // GET: api/Device
        [HttpGet]
        [Authorize(Roles = "Admin")]
        //[ValidateAdminClaim]
        public ActionResult<IEnumerable<DeviceReadDto>> GetAllDevices()
        {
            var devices = _deviceService.GetAllDevices();
            return Ok(devices);
        }

        // PUT: api/Device/{id}
        [HttpPut("{id}/{deviceId}")]
        [ValidateIdClaim]
        public ActionResult<DeviceReadDto> UpdateDevice(string id,string deviceId, DeviceCreateDto deviceUpdateDto)
        {
            var rawId = getRawId(deviceId);
            if(rawId == -1)
            {
                return NotFound();
            }
            _deviceService.UpdateDevice(rawId, deviceUpdateDto);
            var device = _deviceService.GetDeviceById(rawId);
            return Ok(device);
        }

        // DELETE: api/Device/{id}
        [HttpDelete("{id}/{deviceId}")]
        [ValidateIdClaim]
        public ActionResult<DeviceReadDto> DeleteDevice(string id,string deviceId)
        {
            var rawId = getRawId(deviceId);
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
            return Ok(device);
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
