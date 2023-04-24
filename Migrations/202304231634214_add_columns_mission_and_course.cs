namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_columns_mission_and_course : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "TestType", c => c.Int(nullable: false));
            AddColumn("dbo.Missions", "CurrentAction", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Missions", "CurrentAction");
            DropColumn("dbo.Courses", "TestType");
        }
    }
}
