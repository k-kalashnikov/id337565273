using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class ChatTypeConfiguration : IEntityTypeConfiguration<ChatType>
    {
        public void Configure(EntityTypeBuilder<ChatType> builder)
        {
            builder.HasKey(k => new { k.ChatTypeId });

            builder.Property(e => e.ChatTypeId).IsRequired().HasColumnName("ChatTypeID");
            builder.Property(e => e.Mnemonic).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(e => e.Description).HasMaxLength(250).HasColumnType("varchar(250)");
        }
    }
}
