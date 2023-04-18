namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeacherAaddCID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeacherAssessments", "CourseID", c => c.String(maxLength: 128));
            CreateIndex("dbo.TeacherAssessments", "CourseID");
            AddForeignKey("dbo.TeacherAssessments", "CourseID", "dbo.Courses", "CourseID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeacherAssessments", "CourseID", "dbo.Courses");
            DropIndex("dbo.TeacherAssessments", new[] { "CourseID" });
            DropColumn("dbo.TeacherAssessments", "CourseID");
        }
    }
}
