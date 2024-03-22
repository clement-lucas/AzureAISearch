namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// DisclosureScope
    /// </summary>
    public class DisclosureScope
    {
        public int DisclosureScopeId { get; set; }
        public string Name { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
