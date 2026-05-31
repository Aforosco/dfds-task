using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;
using DFDSVisitManagementAPI.Domain.src.Entities.Activities;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.Data;
using Microsoft.EntityFrameworkCore;

namespace DFDSVisitManagementAPI.Business.Services
{
    public class ActivityService : IActivityService
    {
        private readonly AppDbContext _context;

        public ActivityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ActivityResponseDto?> CreateAsync(CreateActivityDto dto)
        {
            var activity = new Activity
            {
                UnitNumber = dto.UnitNumber.ToUpper().Trim(),
                Type = dto.Type,
                CreatedBy = dto.CreatedBy
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return MapToResponse(activity);
        }

        public async Task<ActivityResponseDto?> GetByIdAsync(Guid id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return null;

            return MapToResponse(activity);
        }

        public async Task<IEnumerable<ActivityResponseDto>> GetAllAsync()
        {
            var activities = await _context.Activities.ToListAsync();
            return activities.Select(MapToResponse);
        }

        private static ActivityResponseDto MapToResponse(Activity activity)
        {
            return new ActivityResponseDto
            {
                Id = activity.Id,
                UnitNumber = activity.UnitNumber,
                Type = activity.Type
            };
        }
    }
}