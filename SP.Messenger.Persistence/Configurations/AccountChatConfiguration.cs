using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class AccountChatConfiguration : IEntityTypeConfiguration<AccountChat>
    {
        public void Configure(EntityTypeBuilder<AccountChat> builder)
        {
            builder.HasKey(k => new { k.AccountId, k.ChatId });

            builder.Property(e => e.AccountId).IsRequired().HasColumnName("AccountID");
            builder.Property(e => e.ChatId).IsRequired().HasColumnName("ChatID");
            builder.Property(e => e.UnionUserDate).IsRequired().HasColumnType("date");

            builder.HasOne(e => e.Account)
                .WithMany(e => e.Chats)
                .HasForeignKey(f => f.AccountId)
                .HasConstraintName("FK_Chats_Accounts");

            builder.HasOne(e => e.Chat)
               .WithMany(e => e.Accounts)
               .HasForeignKey(f => f.ChatId)
               .HasConstraintName("FK_Accounts_Chats");
        }
    }
}
