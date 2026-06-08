using DFDSVisitManagementAPI.Domain.src.DTOs.Visits;
using DFDSVisitManagementAPI.Domain.src.DTOs;

namespace DFDSVisitManagementAPI.Business.src.Interfaces
{
    public interface IVisitService
    {
        Task<VisitResponseDto?> CreateAsync(CreateVisitDto dto);
        Task<VisitResponseDto?> GetByIdAsync(Guid id);
        //Task<IEnumerable<VisitResponseDto>> GetAllAsync();
        Task<PagedResponseDto<VisitResponseDto>> GetAllAsync(VisitQueryDto query); 
        Task<VisitResponseDto?> UpdateAsync(Guid id, UpdateVisitDto dto);
    }
}