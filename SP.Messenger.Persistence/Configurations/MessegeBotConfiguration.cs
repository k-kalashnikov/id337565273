using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class MessageBotConfiguration : IEntityTypeConfiguration<MessageBot>
    {
        public void Configure(EntityTypeBuilder<MessageBot> builder)
        {
            builder.ToTable("MessageBots");

            builder.HasKey(e => e.MessageBotId);

            builder.Property(e => e.MessageBotId).HasColumnName("MessageBotID");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(e => e.IsDisabled).IsRequired();

            builder.HasIndex(e => e.Name);
        }
    }
}
