using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.DTOs.Drivers;
using System.Security.Claims;


namespace DFDSVisitManagementAPI.Framework.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDriverDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            dto.CreatedBy = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
                ?? "unknown";

            var result = await _driverService.CreateAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Driver could not be created." });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _driverService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Driver not found." });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _driverService.GetAllAsync();
            return Ok(results);
        }
    }
}