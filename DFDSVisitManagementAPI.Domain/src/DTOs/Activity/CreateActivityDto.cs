using DFDSVisitManagementAPI.Domain.src.Entities.Activities;
using DFDSVisitManagementAPI.Domain.src.Validation;

namespace DFDSVisitManagementAPI.Domain.src.DTOs.Activities
{
    public class CreateActivityDto
    {
        [UpperCaseNoSpaces]
        public required string UnitNumber { get; set; }
        public required ActivityType Type { get; set; }  // ← enum not string
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class ActivityResponseDto
    {
        public Guid Id { get; set; }
        public string UnitNumber { get; set; } = string.Empty;
        public ActivityType Type { get; set; }
    }
}