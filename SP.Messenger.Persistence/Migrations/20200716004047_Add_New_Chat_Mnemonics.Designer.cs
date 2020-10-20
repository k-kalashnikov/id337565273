﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SP.Messenger.Persistence;

namespace SP.Messenger.Persistence.Migrations
{
    [DbContext(typeof(MessengerDbContext))]
    [Migration("20200716004047_Add_New_Chat_Mnemonics")]
    partial class Add_New_Chat_Mnemonics
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Account", b =>
                {
                    b.Property<long>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AccountID")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FirstName")
                        .HasColumnType("varchar(150)");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("MiddleName")
                        .HasColumnType("varchar(150)");

                    b.HasKey("AccountId");

                    b.HasIndex("Login");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.AccountChat", b =>
                {
                    b.Property<long>("AccountId")
                        .HasColumnName("AccountID")
                        .HasColumnType("bigint");

                    b.Property<long>("ChatId")
                        .HasColumnName("ChatID")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UnionUserDate")
                        .HasColumnType("date");

                    b.HasKey("AccountId", "ChatId");

                    b.HasIndex("ChatId");

                    b.ToTable("AccountChat");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Chat", b =>
                {
                    b.Property<long>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ChatID")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ChatTypeId")
                        .HasColumnName("ChatTypeID")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Data")
                        .HasColumnType("jsonb");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255);

                    b.Property<long?>("ParentId")
                        .HasColumnName("ParentID")
                        .HasColumnType("bigint");

                    b.HasKey("ChatId");

                    b.HasIndex("ChatTypeId");

                    b.HasIndex("Data");

                    b.HasIndex("Name");

                    b.HasIndex("ParentId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ChatDocument", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnName("ChatID")
                        .HasColumnType("bigint");

                    b.Property<Guid>("DocumentId")
                        .HasColumnName("DocumentID")
                        .HasColumnType("uuid");

                    b.HasKey("ChatId", "DocumentId");

                    b.HasIndex("DocumentId");

                    b.ToTable("ChatDocument");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ChatType", b =>
                {
                    b.Property<int>("ChatTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ChatTypeID")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("varchar(250)")
                        .HasMaxLength(250);

                    b.Property<string>("Mnemonic")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("ChatTypeId");

                    b.ToTable("ChatTypes");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ChatTypeRoles", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ChatTypeId")
                        .HasColumnName("ChatTypeID")
                        .HasColumnType("integer");

                    b.Property<string>("RoleMnemonic")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ChatTypeId");

                    b.HasIndex("RoleMnemonic");

                    b.ToTable("ChatTypeRoles");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Document", b =>
                {
                    b.Property<Guid>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DocumentID")
                        .HasColumnType("uuid");

                    b.Property<long>("DocumentTypeId")
                        .HasColumnName("DocumentTypeID")
                        .HasColumnType("bigint");

                    b.HasKey("DocumentId");

                    b.HasIndex("DocumentTypeId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.DocumentType", b =>
                {
                    b.Property<long>("DocumentTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DocumentTypeID")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar");

                    b.HasKey("DocumentTypeId");

                    b.HasIndex("Name");

                    b.ToTable("DocumentTypes");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Message", b =>
                {
                    b.Property<long>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MessageID")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("AccountId")
                        .HasColumnName("AccountID")
                        .HasColumnType("bigint");

                    b.Property<long>("ChatId")
                        .HasColumnName("ChatID")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("MessageTypeId")
                        .HasColumnName("MessageTypeID")
                        .HasColumnType("bigint");

                    b.Property<bool>("Modifed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Pined")
                        .HasColumnType("boolean");

                    b.Property<bool>("Readed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ReadedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("RecipientId")
                        .HasColumnName("RecipientID")
                        .HasColumnType("bigint");

                    b.HasKey("MessageId");

                    b.HasIndex("AccountId");

                    b.HasIndex("ChatId");

                    b.HasIndex("MessageTypeId");

                    b.HasIndex("RecipientId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.MessageBot", b =>
                {
                    b.Property<long>("MessageBotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MessageBotID")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("MessageBotId");

                    b.HasIndex("Name");

                    b.ToTable("MessageBots");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.MessageType", b =>
                {
                    b.Property<long>("MessageTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MessageTypeID")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("MessageTypeId");

                    b.HasIndex("Name");

                    b.ToTable("MessageTypes");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ResponseVariant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DecisionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("OrganizationsContent")
                        .HasColumnType("jsonb");

                    b.Property<Guid>("VotingId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("VotingId");

                    b.ToTable("ResponseVariant");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.VotedBy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("AccountId")
                        .HasColumnName("AccountId")
                        .HasColumnType("bigint");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<bool?>("IsLike")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("ResponseVariantId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("VotingId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ResponseVariantId");

                    b.HasIndex("VotingId");

                    b.ToTable("VotedBy");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Voting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CreateBy")
                        .HasColumnName("CreateBy")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Voting");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.AccountChat", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.Account", "Account")
                        .WithMany("Chats")
                        .HasForeignKey("AccountId")
                        .HasConstraintName("FK_Chats_Accounts")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SP.Messenger.Domains.Entities.Chat", "Chat")
                        .WithMany("Accounts")
                        .HasForeignKey("ChatId")
                        .HasConstraintName("FK_Accounts_Chats")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Chat", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.ChatType", "ChatType")
                        .WithMany()
                        .HasForeignKey("ChatTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ChatDocument", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.Chat", "Chat")
                        .WithMany("Documents")
                        .HasForeignKey("ChatId")
                        .HasConstraintName("FK_Chats_Documents")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SP.Messenger.Domains.Entities.Document", "Document")
                        .WithMany("Chats")
                        .HasForeignKey("DocumentId")
                        .HasConstraintName("FK_Documents_Chats")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ChatTypeRoles", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.ChatType", "ChatType")
                        .WithMany()
                        .HasForeignKey("ChatTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Document", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.Message", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SP.Messenger.Domains.Entities.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SP.Messenger.Domains.Entities.MessageType", "MessageType")
                        .WithMany()
                        .HasForeignKey("MessageTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SP.Messenger.Domains.Entities.Account", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId");
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.ResponseVariant", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.Voting", null)
                        .WithMany("ResponseVariants")
                        .HasForeignKey("VotingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SP.Messenger.Domains.Entities.VotedBy", b =>
                {
                    b.HasOne("SP.Messenger.Domains.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SP.Messenger.Domains.Entities.ResponseVariant", "ResponseVariant")
                        .WithMany()
                        .HasForeignKey("ResponseVariantId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SP.Messenger.Domains.Entities.Voting", "Voting")
                        .WithMany("VotedCollection")
                        .HasForeignKey("VotingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
