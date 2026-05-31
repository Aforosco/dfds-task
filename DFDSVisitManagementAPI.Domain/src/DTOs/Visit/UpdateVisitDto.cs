using DFDSVisitManagementAPI.Domain.src.Entities.Visits;

namespace DFDSVisitManagementAPI.Domain.src.DTOs.Visits
{
    public class UpdateVisitDto
    {
        public required VisitStatus Status { get; set; }
        public required string TruckLicensePlate { get; set; }
        public  string UpdatedBy { get; set; } = string.Empty; 

         // For simplicity, we only allow updating the DriverId and not the Activities in this DTO
    }
}