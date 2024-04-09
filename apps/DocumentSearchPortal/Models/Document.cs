namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// Document
    /// </summary>
    public class Document
    {
        public int DocumentId { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public string? metadata_storage_path { get; set; }
        public int CategoryId { get; set; }
        public int ProtocolId { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public int DisclosureScopeId { get; set; }
        public int InformationId { get; set; }
        public Category? Category { get; set; }
        public Protocol? Protocol { get; set; }
        public DisclosureScope? DisclosureScope { get; set; }
        public Information? Information { get; set; }
        public User? CreatedBy { get; set; }
        public User? ModifiedBy { get; set; }
        public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();

    }
}
