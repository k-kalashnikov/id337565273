using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(e => e.AccountId);

            builder.Property(e => e.AccountId).IsRequired().HasColumnName("AccountID");
            builder.Property(e => e.Login).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(e => e.FirstName).HasColumnType("varchar(150)");
            builder.Property(e => e.LastName).HasColumnType("varchar(150)");
            builder.Property(e => e.MiddleName).HasColumnType("varchar(150)");
            builder.Property(e => e.IsDisabled).IsRequired();
            
            builder.HasIndex(e => e.Login);
        }
    }
}
