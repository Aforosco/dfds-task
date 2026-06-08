namespace DFDSVisitManagementAPI.Domain.src.DTOs.Visits
{
    public class VisitQueryDto
    {
        // Filtering
        public string? Status { get; set; }       // e.g. PreRegistered, AtGate
        public string? DriverId { get; set; }     // filter by driver guid

        // Cursor-based pagination
        public string? Cursor { get; set; }       // last seen visit Id (base64 encoded)
        public int PageSize { get; set; } = 20;   // default page size
    }
}