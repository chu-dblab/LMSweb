namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupERModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupERs",
                c => new
                    {
                        RID = c.Int(nullable: false, identity: true),
                        QID = c.Int(nullable: false),
                        Answer = c.String(),
                        Comments = c.String(),
                        GID = c.Int(nullable: false),
                        EvaluatorSID = c.String(),
                        CID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RID)
                .ForeignKey("dbo.Courses", t => t.CID)
                .ForeignKey("dbo.Groups", t => t.GID, cascadeDelete: true)
                .ForeignKey("dbo.Questions", t => t.QID, cascadeDelete: true)
                .Index(t => t.QID)
                .Index(t => t.GID)
                .Index(t => t.CID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupERs", "QID", "dbo.Questions");
            DropForeignKey("dbo.GroupERs", "GID", "dbo.Groups");
            DropForeignKey("dbo.GroupERs", "CourseID", "dbo.Courses");
            DropIndex("dbo.GroupERs", new[] { "CourseID" });
            DropIndex("dbo.GroupERs", new[] { "GID" });
            DropIndex("dbo.GroupERs", new[] { "QID" });
            DropTable("dbo.GroupERs");
        }
    }
}
