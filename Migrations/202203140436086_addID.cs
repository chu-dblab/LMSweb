namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentDraws", "Group_GID", "dbo.Groups");
            DropIndex("dbo.StudentDraws", new[] { "Group_GID" });
            RenameColumn(table: "dbo.StudentDraws", name: "Course_CID", newName: "CourseID");
            RenameColumn(table: "dbo.StudentDraws", name: "Group_GID", newName: "GID");
            RenameColumn(table: "dbo.StudentDraws", name: "Mission_MID", newName: "MID");
            RenameIndex(table: "dbo.StudentDraws", name: "IX_Mission_MID", newName: "IX_MID");
            RenameIndex(table: "dbo.StudentDraws", name: "IX_Course_CID", newName: "IX_CID");
            AlterColumn("dbo.StudentDraws", "GID", c => c.Int(nullable: false));
            CreateIndex("dbo.StudentDraws", "GID");
            AddForeignKey("dbo.StudentDraws", "GID", "dbo.Groups", "GID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentDraws", "GID", "dbo.Groups");
            DropIndex("dbo.StudentDraws", new[] { "GID" });
            AlterColumn("dbo.StudentDraws", "GID", c => c.Int());
            RenameIndex(table: "dbo.StudentDraws", name: "IX_CID", newName: "IX_Course_CID");
            RenameIndex(table: "dbo.StudentDraws", name: "IX_MID", newName: "IX_Mission_MID");
            RenameColumn(table: "dbo.StudentDraws", name: "MID", newName: "Mission_MID");
            RenameColumn(table: "dbo.StudentDraws", name: "GID", newName: "Group_GID");
            RenameColumn(table: "dbo.StudentDraws", name: "CourseID", newName: "Course_CID");
            CreateIndex("dbo.StudentDraws", "Group_GID");
            AddForeignKey("dbo.StudentDraws", "Group_GID", "dbo.Groups", "GID");
        }
    }
}
