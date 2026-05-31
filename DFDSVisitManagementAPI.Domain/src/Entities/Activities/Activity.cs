using DFDSVisitManagementAPI.Domain.src.Entities.Visits;
namespace DFDSVisitManagementAPI.Domain.src.Entities.Activities
{
    public enum ActivityType
    {
        Delivery,
        Collection
    }
    public class Activity : BaseEntity
    {
        public required ActivityType Type { get; set; }
        public required string UnitNumber { get; set; }

       
        public Guid? VisitId { get; set; }
        public Visit? Visit { get; set; }
    }
}