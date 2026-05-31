using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.DTOs.Visits;
using System.Security.Claims;


namespace DFDSVisitManagementAPI.src.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;

        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVisitDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Inject the authenticated user as CreatedBy
            dto.CreatedBy = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
                ?? "unknown";

            var result = await _visitService.CreateAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Driver or Activity not found." });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _visitService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Visit not found." });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _visitService.GetAllAsync();
            return Ok(results);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVisitDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Inject the authenticated user as UpdatedBy
            dto.UpdatedBy = User.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
                ?? "unknown";

            var result = await _visitService.UpdateAsync(id, dto);
            if (result == null) return NotFound(new { message = "Visit not found." });

            return Ok(result);
        }
    }
}