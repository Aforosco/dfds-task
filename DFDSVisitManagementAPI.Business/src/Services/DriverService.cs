using DFDSVisitManagementAPI.Domain.src.DTOs.Drivers;
using DFDSVisitManagementAPI.Domain.src.Entities.Drivers;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.Data;
using Microsoft.EntityFrameworkCore;

namespace DFDSVisitManagementAPI.Business.Services
{
    public class DriverService : IDriverService
    {
        private readonly AppDbContext _context;

        public DriverService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DriverResponseDto?> CreateAsync(CreateDriverDto dto)
        {
            var driver = new Driver
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedBy = dto.CreatedBy
            };

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return MapToResponse(driver);
        }

        public async Task<DriverResponseDto?> GetByIdAsync(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null) return null;

            return MapToResponse(driver);
        }

        public async Task<IEnumerable<DriverResponseDto>> GetAllAsync()
        {
            var drivers = await _context.Drivers.ToListAsync();
            return drivers.Select(MapToResponse);
        }

        private static DriverResponseDto MapToResponse(Driver driver)
        {
            return new DriverResponseDto
            {
                Id = driver.Id,
                FirstName = driver.FirstName,
                LastName = driver.LastName
            };
        }
    }
}