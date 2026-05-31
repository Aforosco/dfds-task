using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;
using DFDSVisitManagementAPI.Domain.src.DTOs.Drivers;
using DFDSVisitManagementAPI.Domain.src.DTOs.Visits;
using DFDSVisitManagementAPI.Domain.src.Entities.Visits;
using DFDSVisitManagementAPI.Domain.src.Entities.Activities;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Domain.src.Data;
using Microsoft.EntityFrameworkCore;

namespace DFDSVisitManagementAPI.Business.src.Services
{
    public class VisitService : IVisitService
    {
        private readonly AppDbContext _context;

        public VisitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VisitResponseDto?> CreateAsync(CreateVisitDto dto)
        {
            var driver = await _context.Drivers.FindAsync(dto.DriverId);
            if (driver == null) return null;

            var visit = new Visit
            {
                Status = dto.Status,
                TruckLicensePlate = dto.TruckLicensePlate.ToUpper().Trim(),
                DriverId = dto.DriverId,
                CreatedBy = dto.CreatedBy,
                Activities = dto.Activities.Select(a => new Activity
                {
                    UnitNumber = a.UnitNumber.ToUpper().Trim(),
                    Type = a.Type,
                    CreatedBy = dto.CreatedBy
                }).ToList()
            };

            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();

            visit.Driver = driver;

            return MapToResponse(visit);
        }

        public async Task<VisitResponseDto?> GetByIdAsync(Guid id)
        {
            var visit = await _context.Visits
                .Include(v => v.Driver)
                .Include(v => v.Activities)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visit == null) return null;

            return MapToResponse(visit);
        }

        public async Task<IEnumerable<VisitResponseDto>> GetAllAsync()
        {
            var visits = await _context.Visits
                .Include(v => v.Driver)
                .Include(v => v.Activities)
                .ToListAsync();

            return visits.Select(MapToResponse);
        }

        public async Task<VisitResponseDto?> UpdateAsync(Guid id, UpdateVisitDto dto)
        {
            var visit = await _context.Visits
                .Include(v => v.Driver)
                .Include(v => v.Activities)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visit == null) return null;

            visit.Status = dto.Status;
            visit.TruckLicensePlate = dto.TruckLicensePlate.ToUpper().Trim();
            visit.MarkUpdated(dto.UpdatedBy);

            await _context.SaveChangesAsync();

            return MapToResponse(visit);
        }

        private static VisitResponseDto MapToResponse(Visit visit)
        {
            return new VisitResponseDto
            {
                Id = visit.Id,
                Status = visit.Status,
                TruckLicensePlate = visit.TruckLicensePlate,
                CreatedAt = visit.CreatedAt,
                CreatedBy = visit.CreatedBy,
                UpdatedAt = visit.UpdatedAt,
                UpdatedBy = visit.UpdatedBy,
                Driver = visit.Driver == null ? null : new DriverResponseDto
                {
                    Id = visit.Driver.Id,
                    FirstName = visit.Driver.FirstName,
                    LastName = visit.Driver.LastName
                },
                Activities = visit.Activities.Select(a => new ActivityResponseDto
                {
                    Id = a.Id,
                    UnitNumber = a.UnitNumber,
                    Type = a.Type
                }).ToList()
            };
        }
    }
}