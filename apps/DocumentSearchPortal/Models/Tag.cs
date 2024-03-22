namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// Tag
    /// </summary>
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; } 
        public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    }
}
