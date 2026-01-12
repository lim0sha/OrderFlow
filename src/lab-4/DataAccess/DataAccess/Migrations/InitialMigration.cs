using DataAccess.Utils.DbUtils;
using FluentMigrator;

namespace DataAccess.Migrations;

[Migration(1, "Initial migration")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        string sql = SqlQueryLoader.Load("V1__Init.sql");
        Execute.Sql(sql);
    }

    public override void Down()
    {
        string sql = SqlQueryLoader.Load("V1__Drop.sql");
        Execute.EmbeddedScript(sql);
    }
}