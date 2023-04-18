namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStudentCode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentCodes",
                c => new
                    {
                        CID = c.String(nullable: false, maxLength: 128),
                        MID = c.String(nullable: false, maxLength: 128),
                        GID = c.Int(nullable: false),
                        CodePath = c.String(),
                        IsEdit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.CID, t.MID, t.GID })
                .ForeignKey("dbo.Courses", t => t.CID, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GID, cascadeDelete: true)
                .ForeignKey("dbo.Missions", t => t.MID, cascadeDelete: true)
                .Index(t => t.CID)
                .Index(t => t.MID)
                .Index(t => t.GID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentCodes", "MID", "dbo.Missions");
            DropForeignKey("dbo.StudentCodes", "GID", "dbo.Groups");
            DropForeignKey("dbo.StudentCodes", "CourseID", "dbo.Courses");
            DropIndex("dbo.StudentCodes", new[] { "GID" });
            DropIndex("dbo.StudentCodes", new[] { "MID" });
            DropIndex("dbo.StudentCodes", new[] { "CourseID" });
            DropTable("dbo.StudentCodes");
        }
    }
}
