namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// Category
    /// </summary>
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
