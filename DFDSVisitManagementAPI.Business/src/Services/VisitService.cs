using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;
using DFDSVisitManagementAPI.Domain.src.DTOs;
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

   
public async Task<PagedResponseDto<VisitResponseDto>> GetAllAsync(VisitQueryDto query)
{
    var dbQuery = _context.Visits
        .Include(v => v.Driver)
        .Include(v => v.Activities)
        .AsQueryable();

    // Filter by status
    if (!string.IsNullOrWhiteSpace(query.Status) &&
        Enum.TryParse<VisitStatus>(query.Status, ignoreCase: true, out var status))
    {
        dbQuery = dbQuery.Where(v => v.Status == status);
    }

    // Filter by driverId
    if (!string.IsNullOrWhiteSpace(query.DriverId) &&
        Guid.TryParse(query.DriverId, out var driverGuid))
    {
        dbQuery = dbQuery.Where(v => v.DriverId == driverGuid);
    }

    // Cursor — decode and apply
    if (!string.IsNullOrWhiteSpace(query.Cursor))
    {
        var cursorId = DecodeCursor(query.Cursor);
        if (cursorId.HasValue)
        {
            // Get visits created after the cursor visit
            var cursorVisit = await _context.Visits
                .Where(v => v.Id == cursorId.Value)
                .Select(v => v.CreatedAt)
                .FirstOrDefaultAsync();

            if (cursorVisit != default)
                dbQuery = dbQuery.Where(v => v.CreatedAt < cursorVisit);
        }
    }

    // Always order consistently for cursor to work
    dbQuery = dbQuery.OrderByDescending(v => v.CreatedAt);

    // Fetch one extra to determine HasMore
    var visits = await dbQuery
        .Take(query.PageSize + 1)
        .ToListAsync();

    var hasMore = visits.Count > query.PageSize;
    if (hasMore) visits.RemoveAt(visits.Count - 1); // remove the extra

    var nextCursor = hasMore
        ? EncodeCursor(visits.Last().Id)
        : null;

    return new PagedResponseDto<VisitResponseDto>
    {
        Data = visits.Select(MapToResponse).ToList(),
        NextCursor = nextCursor,
        HasMore = hasMore,
        PageSize = query.PageSize
    };
}

// Encode Guid to base64 string
private static string EncodeCursor(Guid id)
{
    return Convert.ToBase64String(id.ToByteArray());
}

// Decode base64 string back to Guid
private static Guid? DecodeCursor(string cursor)
{
    try
    {
        var bytes = Convert.FromBase64String(cursor);
        return new Guid(bytes);
    }
    catch
    {
        return null;
    }
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