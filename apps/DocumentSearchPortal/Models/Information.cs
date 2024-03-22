namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// Information
    /// </summary>
    public class Information
    {
        public int InformationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
