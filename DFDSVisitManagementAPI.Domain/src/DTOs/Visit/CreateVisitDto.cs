using DFDSVisitManagementAPI.Domain.src.Entities.Visits;
using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;
using DFDSVisitManagementAPI.Domain.src.Validation;

namespace DFDSVisitManagementAPI.Domain.src.DTOs.Visits
{
    public class CreateVisitDto
    {
        public VisitStatus Status { get; set; } = VisitStatus.PreRegistered;
        [UpperCaseNoSpaces]
        public required string TruckLicensePlate { get; set; }
        public string CreatedBy { get; set; } = string.Empty; 

        // IDs reference existing Driver and Activity records
        public required Guid DriverId { get; set; }
        public List<CreateActivityDto> Activities { get; set; } = new();
    }
}