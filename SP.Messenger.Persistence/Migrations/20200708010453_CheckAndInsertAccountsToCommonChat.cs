using Microsoft.EntityFrameworkCore.Migrations;

namespace SP.Messenger.Persistence.Migrations
{
  public partial class CheckAndInsertAccountsToCommonChat : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
      {
        migrationBuilder.Sql("insert into \"AccountChat\" (\"AccountID\", \"ChatID\", \"UnionUserDate\") " +
          " select a.\"AccountID\", cp.\"ChatID\" , '2020-07-07' " +
          " from \"Accounts\" a, \"chatsparentsview\" cp " +
          "where" +
          "cp.\"DocumentID\" = '07e1136b-74d6-427c-85b2-2791505a8334'" +
          "and" +
          "a.\"AccountID\" not in ( " +
              "select \"AccountID\" from \"AccountChat\" ac" +
              "where \"ChatID\" in (select \"ChatID\" from \"chatsparentsview\" where \"DocumentID\" = '07e1136b-74d6-427c-85b2-2791505a8334')" +
              "and \"AccountID\" = a.\"AccountID\"" +
              "group by \"AccountID\"" +
          ");");


      }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
  }
}
