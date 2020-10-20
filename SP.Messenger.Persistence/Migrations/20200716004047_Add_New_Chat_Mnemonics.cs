using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Messenger.Persistence.Migrations
{
  public partial class Add_New_Chat_Mnemonics : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
      {
        migrationBuilder.Sql("insert into public.\"ChatTypes\"" +
          "(\"ChatTypeID\", \"Mnemonic\", \"Description\") " +
          "values" +
          "(15, 'module.logistic.chat.order', 'Чат заявки')," +
          "(16, 'module.logistic.chat.suborder', 'Чат субзаявки')," +
          "(17, 'module.logistic.chat.suborder.performer', 'Чат исполнителя субзаявки')," +
          "(18, 'module.logistic.chat.suborder.voting', 'Чат комиcсии субзаявки');");
      }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
  }
}
