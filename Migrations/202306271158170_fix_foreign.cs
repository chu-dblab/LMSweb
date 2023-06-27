namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_foreign : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Provideds", "Students_SID", "dbo.Students");
            DropForeignKey("dbo.OptionNews", "QuestionNewID", "dbo.QuestionNews");
            DropIndex("dbo.OptionNews", new[] { "QuestionNewID" });
            DropIndex("dbo.Provideds", new[] { "Students_SID" });
            DropPrimaryKey("dbo.Provideds");
            AddColumn("dbo.Provideds", "UserID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.OptionNews", "QuestionNewID", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.Provideds", new[] { "AID", "UserID" });
            CreateIndex("dbo.Provideds", "UserID");
            CreateIndex("dbo.OptionNews", "QuestionNewID");
            AddForeignKey("dbo.Provideds", "UserID", "dbo.Users", "ID", cascadeDelete: true);
            AddForeignKey("dbo.OptionNews", "QuestionNewID", "dbo.QuestionNews", "QuestionNewID");
            DropColumn("dbo.Provideds", "StudentID");
            DropColumn("dbo.Provideds", "Students_SID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Provideds", "Students_SID", c => c.String(maxLength: 128));
            AddColumn("dbo.Provideds", "StudentID", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.OptionNews", "QuestionNewID", "dbo.QuestionNews");
            DropForeignKey("dbo.Provideds", "UserID", "dbo.Users");
            DropIndex("dbo.OptionNews", new[] { "QuestionNewID" });
            DropIndex("dbo.Provideds", new[] { "UserID" });
            DropPrimaryKey("dbo.Provideds");
            AlterColumn("dbo.OptionNews", "QuestionNewID", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Provideds", "UserID");
            AddPrimaryKey("dbo.Provideds", new[] { "AID", "StudentID" });
            CreateIndex("dbo.Provideds", "Students_SID");
            CreateIndex("dbo.OptionNews", "QuestionNewID");
            AddForeignKey("dbo.OptionNews", "QuestionNewID", "dbo.QuestionNews", "QuestionNewID", cascadeDelete: true);
            AddForeignKey("dbo.Provideds", "Students_SID", "dbo.Students", "SID");
        }
    }
}
