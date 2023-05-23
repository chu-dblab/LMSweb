namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExecutionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Executions",
                c => new
                    {
                        GID = c.Int(nullable: false),
                        MID = c.String(nullable: false, maxLength: 128),
                        CurrentAction = c.String(),
                    })
                .PrimaryKey(t => new { t.GID, t.MID })
                .ForeignKey("dbo.Groups", t => t.GID, cascadeDelete: true)
                .ForeignKey("dbo.Missions", t => t.MID, cascadeDelete: true)
                .Index(t => t.GID)
                .Index(t => t.MID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Executions", "MID", "dbo.Missions");
            DropForeignKey("dbo.Executions", "GID", "dbo.Groups");
            DropIndex("dbo.Executions", new[] { "MID" });
            DropIndex("dbo.Executions", new[] { "GID" });
            DropTable("dbo.Executions");
        }
    }
}
