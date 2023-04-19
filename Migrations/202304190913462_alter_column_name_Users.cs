namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alter_column_name_Users : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Users");
            AddColumn("dbo.Users", "ID", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Users", "UPassword", c => c.String(nullable: false));
            AddPrimaryKey("dbo.Users", "ID");
            DropColumn("dbo.Users", "UID");
            DropColumn("dbo.Users", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false));
            AddColumn("dbo.Users", "UID", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.Users");
            DropColumn("dbo.Users", "UPassword");
            DropColumn("dbo.Users", "ID");
            AddPrimaryKey("dbo.Users", "UID");
        }
    }
}
