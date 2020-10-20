using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Messenger.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Persistence.Configurations
{
    public class ChatTypeRolesConfiguration
        : IEntityTypeConfiguration<ChatTypeRoles>
    {
        public void Configure(EntityTypeBuilder<ChatTypeRoles> builder)
        {
            builder.ToTable("ChatTypeRoles");

            builder.HasKey(k => new { k.Id });

            builder.Property(e => e.ChatTypeId)
                .IsRequired()
                .HasColumnName("ChatTypeID");

            builder.Property(e => e.RoleMnemonic)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.HasIndex(x => x.RoleMnemonic);
        }
    }
}
