using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MonitoringComService.Services;

[ApiController]
[Route("[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly MeasurementService _measurementService;

    public MeasurementController(MeasurementService measurementService)
    {
        _measurementService = measurementService;
    }

    [HttpGet("{deviceId}/{date}")]
    public async Task<IActionResult> GetHourlyConsumption(string deviceId, DateTime date)
    {
        try
        {
            var hourlyConsumption = await _measurementService.GetHourlyMeasurementsByDeviceAndDateAsync(deviceId, date);
            return Ok(hourlyConsumption);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); 
        }
        catch (Exception ex)
        {
            // other exceptions
            return StatusCode(500, "An error occurred while processing your request. " + ex.Message);
        }
    }
}
