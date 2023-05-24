namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumnCurrentActionToCurrentStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Executions", "CurrentStatus", c => c.String());
            DropColumn("dbo.Executions", "CurrentAction");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Executions", "CurrentAction", c => c.String());
            DropColumn("dbo.Executions", "CurrentStatus");
        }
    }
}
