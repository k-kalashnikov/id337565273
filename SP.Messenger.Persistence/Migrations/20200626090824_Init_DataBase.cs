using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SP.Messenger.Persistence.Migrations
{
    public partial class Init_DataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(150)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "varchar(150)", maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(type: "varchar(150)", maxLength: 50, nullable: true),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "ChatTypes",
                columns: table => new
                {
                    ChatTypeID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Mnemonic = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatTypes", x => x.ChatTypeID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    DocumentTypeID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar", nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.DocumentTypeID);
                });

            migrationBuilder.CreateTable(
                name: "MessageBots",
                columns: table => new
                {
                    MessageBotID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBots", x => x.MessageBotID);
                });

            migrationBuilder.CreateTable(
                name: "MessageTypes",
                columns: table => new
                {
                    MessageTypeID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTypes", x => x.MessageTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Voting",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateBy = table.Column<long>(nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    IsClosed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false),
                    ChatTypeID = table.Column<int>(nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: true),
                    CreateAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatID);
                    table.ForeignKey(
                        name: "FK_Chats_ChatTypes_ChatTypeID",
                        column: x => x.ChatTypeID,
                        principalTable: "ChatTypes",
                        principalColumn: "ChatTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatTypeRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatTypeID = table.Column<int>(nullable: false),
                    RoleMnemonic = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatTypeRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatTypeRoles_ChatTypes_ChatTypeID",
                        column: x => x.ChatTypeID,
                        principalTable: "ChatTypes",
                        principalColumn: "ChatTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentTypes_DocumentTypeID",
                        column: x => x.DocumentTypeID,
                        principalTable: "DocumentTypes",
                        principalColumn: "DocumentTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseVariant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DecisionId = table.Column<Guid>(nullable: true),
                    VotingId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    OrganizationsContent = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseVariant_Voting_VotingId",
                        column: x => x.VotingId,
                        principalTable: "Voting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountChat",
                columns: table => new
                {
                    ChatID = table.Column<long>(nullable: false),
                    AccountID = table.Column<long>(nullable: false),
                    UnionUserDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountChat", x => new { x.AccountID, x.ChatID });
                    table.ForeignKey(
                        name: "FK_Chats_Accounts",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Chats",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ChatID = table.Column<long>(nullable: false),
                    AccountID = table.Column<long>(nullable: false),
                    RecipientID = table.Column<long>(nullable: true),
                    Pined = table.Column<bool>(nullable: false),
                    MessageTypeID = table.Column<long>(nullable: false),
                    Readed = table.Column<bool>(nullable: false),
                    ReadedDate = table.Column<DateTime>(nullable: true),
                    Modifed = table.Column<bool>(nullable: false),
                    ModifedDate = table.Column<DateTime>(nullable: true),
                    Content = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_MessageTypes_MessageTypeID",
                        column: x => x.MessageTypeID,
                        principalTable: "MessageTypes",
                        principalColumn: "MessageTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Accounts_RecipientID",
                        column: x => x.RecipientID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatDocument",
                columns: table => new
                {
                    ChatID = table.Column<long>(nullable: false),
                    DocumentID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatDocument", x => new { x.ChatID, x.DocumentID });
                    table.ForeignKey(
                        name: "FK_Chats_Documents",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_Chats",
                        column: x => x.DocumentID,
                        principalTable: "Documents",
                        principalColumn: "DocumentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotedBy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VotingId = table.Column<Guid>(nullable: false),
                    ResponseVariantId = table.Column<Guid>(nullable: true),
                    AccountId = table.Column<long>(nullable: false),
                    IsLike = table.Column<bool>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotedBy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotedBy_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotedBy_ResponseVariant_ResponseVariantId",
                        column: x => x.ResponseVariantId,
                        principalTable: "ResponseVariant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotedBy_Voting_VotingId",
                        column: x => x.VotingId,
                        principalTable: "Voting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountChat_ChatID",
                table: "AccountChat",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Login",
                table: "Accounts",
                column: "Login");

            migrationBuilder.CreateIndex(
                name: "IX_ChatDocument_DocumentID",
                table: "ChatDocument",
                column: "DocumentID");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ChatTypeID",
                table: "Chats",
                column: "ChatTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Data",
                table: "Chats",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Name",
                table: "Chats",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ParentID",
                table: "Chats",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatTypeRoles_ChatTypeID",
                table: "ChatTypeRoles",
                column: "ChatTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatTypeRoles_RoleMnemonic",
                table: "ChatTypeRoles",
                column: "RoleMnemonic");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeID",
                table: "Documents",
                column: "DocumentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Name",
                table: "DocumentTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBots_Name",
                table: "MessageBots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AccountID",
                table: "Messages",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatID",
                table: "Messages",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageTypeID",
                table: "Messages",
                column: "MessageTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientID",
                table: "Messages",
                column: "RecipientID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTypes_Name",
                table: "MessageTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseVariant_VotingId",
                table: "ResponseVariant",
                column: "VotingId");

            migrationBuilder.CreateIndex(
                name: "IX_VotedBy_AccountId",
                table: "VotedBy",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VotedBy_ResponseVariantId",
                table: "VotedBy",
                column: "ResponseVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_VotedBy_VotingId",
                table: "VotedBy",
                column: "VotingId");

            migrationBuilder.CreateIndex(
                name: "IX_Voting_Name",
                table: "Voting",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountChat");

            migrationBuilder.DropTable(
                name: "ChatDocument");

            migrationBuilder.DropTable(
                name: "ChatTypeRoles");

            migrationBuilder.DropTable(
                name: "MessageBots");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "VotedBy");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "MessageTypes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ResponseVariant");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "ChatTypes");

            migrationBuilder.DropTable(
                name: "Voting");
        }
    }
}
