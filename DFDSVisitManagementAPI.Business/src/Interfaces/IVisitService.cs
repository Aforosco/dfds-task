using DFDSVisitManagementAPI.Domain.src.DTOs.Visits;

namespace DFDSVisitManagementAPI.Business.src.Interfaces
{
    public interface IVisitService
    {
        Task<VisitResponseDto?> CreateAsync(CreateVisitDto dto);
        Task<VisitResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<VisitResponseDto>> GetAllAsync();
        Task<VisitResponseDto?> UpdateAsync(Guid id, UpdateVisitDto dto);
    }
}