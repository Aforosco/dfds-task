using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;

namespace DFDSVisitManagementAPI.Business.src.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityResponseDto?> CreateAsync(CreateActivityDto dto);
        Task<ActivityResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ActivityResponseDto>> GetAllAsync();
    }
}