namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<Document> CreatedDocuments { get; set; } = new List<Document>();
        public ICollection<Document> ModifiedDocuments { get; set; } = new List<Document>();
    }

}
