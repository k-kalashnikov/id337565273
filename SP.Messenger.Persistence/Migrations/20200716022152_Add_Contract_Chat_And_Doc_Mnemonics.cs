using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Messenger.Persistence.Migrations
{
  public partial class Add_Contract_Chat_And_Doc_Mnemonics : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.Sql("INSERT INTO public.\"ChatTypes\" " +
        "(\"ChatTypeID\", \"Mnemonic\", \"Description\") " +
        "VALUES(19, 'module.contract.chat.contract', 'Чат договора');");

      migrationBuilder.Sql("INSERT INTO public.\"DocumentTypes\" " +
        "(\"DocumentTypeID\", \"Name\", \"IsDisabled\") " +
        "VALUES(8, 'Contract', false);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
  }
}
