using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("Chats");

            builder.HasKey(e => e.ChatId);

            builder.Property(e => e.ChatId).HasColumnName("ChatID").HasColumnType("bigint");
            builder.Property(e => e.ParentId).HasColumnName("ParentID").HasColumnType("bigint");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(e => e.ChatTypeId).HasColumnName("ChatTypeID");
            builder.Property(e => e.IsDisabled).IsRequired();
            builder.Property(e => e.Data).HasColumnType("jsonb");

            builder.HasIndex(e => e.Name);
            builder.HasIndex(e => e.ParentId);
            builder.HasIndex(e => e.Data);
        }
    }
}
