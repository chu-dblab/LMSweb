namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupCID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "CourseID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "CourseID");
        }
    }
}
