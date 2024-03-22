namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// vw_DocumentsDetails
    /// </summary>
    public class vw_DocumentsDetails
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public string? metadata_storage_path { get; set; }
        public string? CategoryName { get; set; }
        public string? ProtocolName { get; set; }
        public string? CreatedByName { get; set; }
        public string? ModifiedByName { get; set; }
        public string? DisclosureScopeName { get; set; }
        public string? InformationName { get; set; }
    }
}