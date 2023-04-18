namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delQuestionCID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Questions", name: "CourseID", newName: "Course_CID");
            RenameIndex(table: "dbo.Questions", name: "IX_CID", newName: "IX_Course_CID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Questions", name: "IX_Course_CID", newName: "IX_CID");
            RenameColumn(table: "dbo.Questions", name: "Course_CID", newName: "CourseID");
        }
    }
}
