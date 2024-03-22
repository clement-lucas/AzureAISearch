namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// Protocol
    /// </summary>
    public class Protocol
    {
        public int ProtocolId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
