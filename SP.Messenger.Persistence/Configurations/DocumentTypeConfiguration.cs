using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
    {
        public void Configure(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("DocumentTypes");

            builder.HasKey(e => e.DocumentTypeId);

            builder.Property(e => e.DocumentTypeId).HasColumnName("DocumentTypeID");
            builder.Property(e => e.Name).IsRequired().HasColumnType("varchar");
            builder.Property(e => e.IsDisabled).IsRequired();

            builder.HasIndex(e => e.Name);
        }
    }
}
