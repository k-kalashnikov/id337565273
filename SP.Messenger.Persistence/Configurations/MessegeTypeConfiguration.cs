using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class MessageTypeConfiguration : IEntityTypeConfiguration<MessageType>
    {
        public void Configure(EntityTypeBuilder<MessageType> builder)
        {
            builder.ToTable("MessageTypes");

            builder.HasKey(e => e.MessageTypeId);

            builder.Property(e => e.MessageTypeId).HasColumnName("MessageTypeID");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(e => e.IsDisabled).IsRequired();
            
            builder.HasIndex(e => e.Name);
        }
    }
}
