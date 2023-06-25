namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentID = c.String(nullable: false, maxLength: 128),
                        CContent = c.String(),
                        StudentID = c.String(),
                        CommentTypes_CommentTypeID = c.String(maxLength: 128),
                        Student_SID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.CommentTypes", t => t.CommentTypes_CommentTypeID)
                .ForeignKey("dbo.Students", t => t.Student_SID)
                .Index(t => t.CommentTypes_CommentTypeID)
                .Index(t => t.Student_SID);
            
            CreateTable(
                "dbo.CommentTypes",
                c => new
                    {
                        CommentTypeID = c.String(nullable: false, maxLength: 128),
                        CTContent = c.String(),
                        CTAttribute = c.String(),
                    })
                .PrimaryKey(t => t.CommentTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Student_SID", "dbo.Students");
            DropForeignKey("dbo.Comments", "CommentTypes_CommentTypeID", "dbo.CommentTypes");
            DropIndex("dbo.Comments", new[] { "Student_SID" });
            DropIndex("dbo.Comments", new[] { "CommentTypes_CommentTypeID" });
            DropTable("dbo.CommentTypes");
            DropTable("dbo.Comments");
        }
    }
}
