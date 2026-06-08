using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.DTOs.Visits;
using System.Security.Claims;
using DFDSVisitManagementAPI.Domain.src.DTOs;


namespace DFDSVisitManagementAPI.src.Application.Controllers
{
    /// <summary>
   /// Manages truck visit records at the terminal
  /// </summary>
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

        /// <summary>
        /// Creates a new visit record
        /// </summary>
        /// <param name="dto">Visit details including driver and activities</param>
        /// <returns>Created visit record</returns>
        /// <response code="201">Visit created successfully</response>
        /// <response code="400">Driver not found or invalid data</response>
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
        /// <summary>
        /// Retrieves a single visit by ID
        /// </summary>
        /// <param name="id">Visit GUID</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _visitService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Visit not found." });

            return Ok(result);
        }


        /// <summary>
        /// Retrieves all visit records with optional filtering and cursor-based pagination.
        /// </summary>
        /// <param name="query">Filter by status/driverId. Use cursor for pagination.</param>
        /// <returns>Paged list of visits</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponseDto<VisitResponseDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] VisitQueryDto query)
        {
            var results = await _visitService.GetAllAsync(query);
            return Ok(results);
        }

      

        /// <summary>
        /// Updates the status of an existing visit
        /// </summary>
        /// <param name="id">Visit GUID</param>
        /// <param name="dto">Updated status and license plate</param>
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