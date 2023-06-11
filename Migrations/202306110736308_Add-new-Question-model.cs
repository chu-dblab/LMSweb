namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddnewQuestionmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Provideds",
                c => new
                    {
                        AID = c.String(nullable: false, maxLength: 128),
                        StudentID = c.String(nullable: false, maxLength: 128),
                        Students_SID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AID, t.StudentID })
                .ForeignKey("dbo.Answers", t => t.AID, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.Students_SID)
                .Index(t => t.AID)
                .Index(t => t.Students_SID);
            
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        AID = c.String(nullable: false, maxLength: 128),
                        AContent = c.String(),
                        ATime = c.DateTime(nullable: false),
                        QuestionNewID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AID)
                .ForeignKey("dbo.QuestionNews", t => t.QuestionNewID)
                .Index(t => t.QuestionNewID);
            
            CreateTable(
                "dbo.QuestionNews",
                c => new
                    {
                        QuestionNewID = c.String(nullable: false, maxLength: 128),
                        Content = c.String(),
                        QType = c.String(),
                        EProcedureID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.QuestionNewID)
                .ForeignKey("dbo.ExperimentalProcedures", t => t.EProcedureID)
                .Index(t => t.EProcedureID);
            
            CreateTable(
                "dbo.ExperimentalProcedures",
                c => new
                    {
                        EProcedureID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.EProcedureID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Provideds", "Students_SID", "dbo.Students");
            DropForeignKey("dbo.Answers", "QuestionNewID", "dbo.QuestionNews");
            DropForeignKey("dbo.QuestionNews", "EProcedureID", "dbo.ExperimentalProcedures");
            DropForeignKey("dbo.Provideds", "AID", "dbo.Answers");
            DropIndex("dbo.QuestionNews", new[] { "EProcedureID" });
            DropIndex("dbo.Answers", new[] { "QuestionNewID" });
            DropIndex("dbo.Provideds", new[] { "Students_SID" });
            DropIndex("dbo.Provideds", new[] { "AID" });
            DropTable("dbo.ExperimentalProcedures");
            DropTable("dbo.QuestionNews");
            DropTable("dbo.Answers");
            DropTable("dbo.Provideds");
        }
    }
}
