using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class ResponseVariantConfiguration : IEntityTypeConfiguration<ResponseVariant>
    {
        public void Configure(EntityTypeBuilder<ResponseVariant> builder)
        {
            builder.ToTable(nameof(ResponseVariant));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.VotingId)
                .IsRequired();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            builder.Property(e=>e.OrganizationsContent)
                .HasColumnType("jsonb");

            builder.HasIndex(e => e.VotingId);
        }
    }
}
