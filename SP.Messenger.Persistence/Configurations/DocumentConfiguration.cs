using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(e => e.DocumentId);

            builder.Property(e => e.DocumentId).IsRequired().HasColumnType("uuid").HasColumnName("DocumentID");
            builder.Property(e => e.DocumentTypeId).IsRequired().HasColumnName("DocumentTypeID");

            builder.HasIndex(i => i.DocumentTypeId);
        }
    }
}
