namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEvaluationModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EvalutionResponses",
                c => new
                    {
                        RID = c.Int(nullable: false, identity: true),
                        QID = c.Int(nullable: false),
                        Answer = c.String(),
                        Comments = c.String(),
                        SID = c.String(maxLength: 128),
                        CID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RID)
                .ForeignKey("dbo.Courses", t => t.CID)
                .ForeignKey("dbo.Questions", t => t.QID, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.SID)
                .Index(t => t.QID)
                .Index(t => t.SID)
                .Index(t => t.CID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EvalutionResponses", "SID", "dbo.Students");
            DropForeignKey("dbo.EvalutionResponses", "QID", "dbo.Questions");
            DropForeignKey("dbo.EvalutionResponses", "CourseID", "dbo.Courses");
            DropIndex("dbo.EvalutionResponses", new[] { "CourseID" });
            DropIndex("dbo.EvalutionResponses", new[] { "SID" });
            DropIndex("dbo.EvalutionResponses", new[] { "QID" });
            DropTable("dbo.EvalutionResponses");
        }
    }
}
