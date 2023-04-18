namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createSurveyModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QID = c.String(nullable: false, maxLength: 128),
                        Type = c.String(),
                        Description = c.String(),
                        CID = c.String(maxLength: 128),
                        MID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.QID)
                .ForeignKey("dbo.Courses", t => t.CID)
                .ForeignKey("dbo.Missions", t => t.MID)
                .Index(t => t.CID)
                .Index(t => t.MID);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 128),
                        QID = c.String(maxLength: 128),
                        Options = c.String(),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("dbo.Questions", t => t.QID)
                .Index(t => t.QID);
            
            CreateTable(
                "dbo.Responses",
                c => new
                    {
                        RID = c.String(nullable: false, maxLength: 128),
                        QID = c.String(maxLength: 128),
                        Answer = c.String(),
                        SID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RID)
                .ForeignKey("dbo.Questions", t => t.QID)
                .ForeignKey("dbo.Students", t => t.SID)
                .Index(t => t.QID)
                .Index(t => t.SID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Responses", "SID", "dbo.Students");
            DropForeignKey("dbo.Responses", "QID", "dbo.Questions");
            DropForeignKey("dbo.Options", "QID", "dbo.Questions");
            DropForeignKey("dbo.Questions", "MID", "dbo.Missions");
            DropForeignKey("dbo.Questions", "CourseID", "dbo.Courses");
            DropIndex("dbo.Responses", new[] { "SID" });
            DropIndex("dbo.Responses", new[] { "QID" });
            DropIndex("dbo.Options", new[] { "QID" });
            DropIndex("dbo.Questions", new[] { "MID" });
            DropIndex("dbo.Questions", new[] { "CourseID" });
            DropTable("dbo.Responses");
            DropTable("dbo.Options");
            DropTable("dbo.Questions");
        }
    }
}
