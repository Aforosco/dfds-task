namespace DFDSVisitManagementAPI.Domain.src.DTOs
{
    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();
        public string? NextCursor { get; set; }  
        public bool HasMore { get; set; }
        public int PageSize { get; set; }
    }
}