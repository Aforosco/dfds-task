using DFDSVisitManagementAPI.Domain.src.Entities;
using DFDSVisitManagementAPI.Domain.src.Entities.Drivers;
using DFDSVisitManagementAPI.Domain.src.Entities.Activities;

namespace DFDSVisitManagementAPI.Domain.src.Entities.Visits
{
    public enum VisitStatus
    {
        PreRegistered,
        AtGate,
        Onsite,
        Completed,
    }

    public class Visit : BaseEntity
    {
    
        public VisitStatus Status { get; set; } = VisitStatus.PreRegistered;

          public required string TruckLicensePlate { get; set; }
        // FK properties
        public Guid DriverId { get; set; }
        public Guid ActivityId { get; set; }

        // Navigation properties — proper entity types, not strings
        public Driver? Driver { get; set; }
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}