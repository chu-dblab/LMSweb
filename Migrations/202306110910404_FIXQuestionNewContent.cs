namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FIXQuestionNewContent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuestionNews", "QContent", c => c.String());
            DropColumn("dbo.QuestionNews", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QuestionNews", "Content", c => c.String());
            DropColumn("dbo.QuestionNews", "QContent");
        }
    }
}
