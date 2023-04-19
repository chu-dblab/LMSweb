namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alter_column_role_rolename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "RoleName", c => c.String(nullable: false));
            DropColumn("dbo.Users", "Role");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Role", c => c.String(nullable: false));
            DropColumn("dbo.Users", "RoleName");
        }
    }
}
