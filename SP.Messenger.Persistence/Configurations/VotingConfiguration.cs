using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class VotingConfiguration : IEntityTypeConfiguration<Voting>
    {
        public void Configure(EntityTypeBuilder<Voting> builder)
        {
            builder.ToTable(nameof(Voting));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreateBy)
                .HasColumnName("CreateBy");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            builder.Property(e => e.IsClosed)
                .IsRequired();

            var navigationResponseVariants =
              builder.Metadata.FindNavigation(nameof(Voting.ResponseVariants));
            navigationResponseVariants.SetPropertyAccessMode(PropertyAccessMode.Field);

            var navigationVotedCollection =
              builder.Metadata.FindNavigation(nameof(Voting.VotedCollection));
            navigationVotedCollection.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(e => e.Name);
        }
    }
}
