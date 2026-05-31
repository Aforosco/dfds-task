using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;
using System.Security.Claims;


namespace DFDSVisitManagementAPI.src.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActivityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            dto.CreatedBy = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
                ?? "unknown";

            var result = await _activityService.CreateAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Activity could not be created." });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _activityService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Activity not found." });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _activityService.GetAllAsync();
            return Ok(results);
        }
    }
}