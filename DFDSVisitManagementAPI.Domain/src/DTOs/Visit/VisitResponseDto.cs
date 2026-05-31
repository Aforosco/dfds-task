using DFDSVisitManagementAPI.Domain.src.Entities.Visits;
using DFDSVisitManagementAPI.Domain.src.DTOs.Drivers;
using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;

namespace DFDSVisitManagementAPI.Domain.src.DTOs.Visits
{
    public class VisitResponseDto
    {
        public Guid Id { get; set; }
        public VisitStatus Status { get; set; }
        public string TruckLicensePlate { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
         public string CreatedBy { get; set; } = string.Empty; 
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Nested response DTOs — not just IDs
        public DriverResponseDto? Driver { get; set; }
        public List<ActivityResponseDto> Activities { get; set; } = new();
    }
}