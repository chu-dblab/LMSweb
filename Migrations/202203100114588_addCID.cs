namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Questions", name: "Course_CID", newName: "CourseID");
            RenameIndex(table: "dbo.Questions", name: "IX_Course_CID", newName: "IX_CID");
            AddColumn("dbo.PeerAssessments", "CourseID", c => c.String(maxLength: 128));
            AddColumn("dbo.Responses", "CourseID", c => c.String(maxLength: 128));
            CreateIndex("dbo.PeerAssessments", "CourseID");
            CreateIndex("dbo.Responses", "CourseID");
            AddForeignKey("dbo.PeerAssessments", "CourseID", "dbo.Courses", "CourseID");
            AddForeignKey("dbo.Responses", "CourseID", "dbo.Courses", "CourseID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Responses", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.PeerAssessments", "CourseID", "dbo.Courses");
            DropIndex("dbo.Responses", new[] { "CourseID" });
            DropIndex("dbo.PeerAssessments", new[] { "CourseID" });
            DropColumn("dbo.Responses", "CourseID");
            DropColumn("dbo.PeerAssessments", "CourseID");
            RenameIndex(table: "dbo.Questions", name: "IX_CID", newName: "IX_Course_CID");
            RenameColumn(table: "dbo.Questions", name: "CourseID", newName: "Course_CID");
        }
    }
}
