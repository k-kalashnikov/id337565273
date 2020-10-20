using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class ChatDocumentConfiguration : IEntityTypeConfiguration<ChatDocument>
    {
        public void Configure(EntityTypeBuilder<ChatDocument> builder)
        {
            builder.HasKey(k => new { k.ChatId, k.DocumentId });

            builder.Property(e => e.ChatId).IsRequired().HasColumnName("ChatID");
            builder.Property(e => e.DocumentId).IsRequired().HasColumnName("DocumentID");

            builder.HasOne(e => e.Chat)
                .WithMany(e => e.Documents)
                .HasForeignKey(f => f.ChatId)
                .HasConstraintName("FK_Chats_Documents");

            builder.HasOne(e => e.Document)
               .WithMany(e => e.Chats)
               .HasForeignKey(f => f.DocumentId)
               .HasConstraintName("FK_Documents_Chats");
        }
    }
}
