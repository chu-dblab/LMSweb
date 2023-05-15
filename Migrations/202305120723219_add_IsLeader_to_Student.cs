namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_IsLeader_to_Student : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "IsLeader", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "IsLeader");
        }
    }
}
