namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuestionNewFKCID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuestionNews", "CID", c => c.String(maxLength: 128));
            CreateIndex("dbo.QuestionNews", "CID");
            AddForeignKey("dbo.QuestionNews", "CID", "dbo.Courses", "CID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionNews", "CID", "dbo.Courses");
            DropIndex("dbo.QuestionNews", new[] { "CID" });
            DropColumn("dbo.QuestionNews", "CID");
        }
    }
}
