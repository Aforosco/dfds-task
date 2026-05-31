using DFDSVisitManagementAPI.Domain.src.DTOs.Drivers;

namespace DFDSVisitManagementAPI.Business.src.Interfaces
{
    public interface IDriverService
    {
        Task<DriverResponseDto?> CreateAsync(CreateDriverDto dto);
        Task<DriverResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<DriverResponseDto>> GetAllAsync();
    }
}