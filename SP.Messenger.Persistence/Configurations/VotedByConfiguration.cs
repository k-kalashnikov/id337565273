using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class VotedByConfiguration : IEntityTypeConfiguration<VotedBy>
    {
        public void Configure(EntityTypeBuilder<VotedBy> builder)
        {
            builder.ToTable(nameof(VotedBy));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.VotingId)
                .IsRequired();

            builder.Property(e => e.ResponseVariantId)
                .IsRequired(false);

            builder.Property(e => e.AccountId)
                .HasColumnName("AccountId")
                .IsRequired();

            builder.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey("AccountId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(e => e.Voting)
                .WithMany(v => v.VotedCollection)
                .HasForeignKey("VotingId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(e => e.ResponseVariant)
                .WithMany()
                .HasForeignKey("ResponseVariantId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
