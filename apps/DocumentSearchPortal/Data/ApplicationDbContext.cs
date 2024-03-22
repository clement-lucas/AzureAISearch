using Microsoft.EntityFrameworkCore;
using DocumentSearchPortal.Models;  

namespace DocumentSearchPortal.Data
{
    /// <summary>
    /// ApplicationDbContext
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for entities.
        public DbSet<Document> Documents { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Information> Information { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<DisclosureScope> DisclosureScopes { get; set; }

        public DbSet<Protocol> Protocols { get; set; }

        public DbSet<vw_DocumentsDetails> vw_DocumentsDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure vw_DocumentsDetails as a keyless entity type  
            modelBuilder.Entity<vw_DocumentsDetails>().HasNoKey();

            // Example of configuring relationships  
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Category)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CategoryId);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Information)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.InformationId);

            modelBuilder.Entity<Document>()
            .HasOne(d => d.Protocol)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ProtocolId);

            // Configure the relationship between Document and CreatedBy User  
            modelBuilder.Entity<Document>()
                .HasOne(d => d.CreatedBy) // Document has one CreatedBy User  
                .WithMany(u => u.CreatedDocuments) // User has many CreatedDocuments  
                .HasForeignKey(d => d.CreatedById) // ForeignKey in Document entity  
                .OnDelete(DeleteBehavior.Restrict); // Configure the delete behavior as needed

            // Configure the relationship between Document and ModifiedBy User  
            modelBuilder.Entity<Document>()
                .HasOne(d => d.ModifiedBy) // Document has one ModifiedBy User  
                .WithMany(u => u.ModifiedDocuments) // User has many ModifiedDocuments  
                .HasForeignKey(d => d.ModifiedById) // ForeignKey in Document entity  
                .OnDelete(DeleteBehavior.Restrict); // Configure the delete behavior as needed 

            // Configuring the many-to-many relationship between Document and Tag  
            modelBuilder.Entity<DocumentTag>()
                .HasKey(dt => new { dt.DocumentId, dt.TagId });

            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.Document)
                .WithMany(d => d.DocumentTags)
                .HasForeignKey(dt => dt.DocumentId);

            modelBuilder.Entity<DocumentTag>()
                .HasOne(dt => dt.Tag)
                .WithMany(t => t.DocumentTags)
                .HasForeignKey(dt => dt.TagId);
        }
    }
}
