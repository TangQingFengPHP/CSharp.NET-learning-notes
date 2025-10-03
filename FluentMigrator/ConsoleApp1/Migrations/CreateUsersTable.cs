using FluentMigrator;

namespace ConsoleApp1.Migrations;

[Migration(202510031534)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Username").AsString(100).NotNullable().WithDefaultValue("")
            .WithColumn("Email").AsString(100).NotNullable().WithDefaultValue("")
            .WithColumn("CreatedAt").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);
    }
    
    public override void Down()
    {
        Delete.Table("Users");
    }
}