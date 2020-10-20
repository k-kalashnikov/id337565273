using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(e => e.MessageId);

            builder.Property(e => e.MessageId).IsRequired().HasColumnType("bigint").HasColumnName("MessageID");
            builder.Property(e => e.Content).IsRequired().HasColumnType("jsonb");
            builder.Property(e => e.MessageTypeId).IsRequired().HasColumnName("MessageTypeID");
            builder.Property(e => e.ChatId).IsRequired().HasColumnName("ChatID");
            builder.Property(e => e.AccountId).IsRequired().HasColumnName("AccountID");
            builder.Property(e => e.RecipientId).HasColumnName("RecipientID");
            
            builder.HasIndex(i => i.MessageTypeId);
            builder.HasIndex(i => i.ChatId);
            builder.HasIndex(i => i.AccountId);
            builder.HasIndex(i => i.RecipientId);
        }
    }
}
