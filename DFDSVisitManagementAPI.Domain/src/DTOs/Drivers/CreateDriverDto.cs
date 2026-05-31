namespace DFDSVisitManagementAPI.Domain.src.DTOs.Drivers
{
    public class CreateDriverDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
         public string CreatedBy { get; set; } = string.Empty; 
    }

    public class DriverResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}