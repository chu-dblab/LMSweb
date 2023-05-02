namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alter_col_student_gid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "group_GID", "dbo.Groups");
            DropIndex("dbo.Students", new[] { "group_GID" });
            RenameColumn(table: "dbo.Students", name: "group_GID", newName: "GID");
            AlterColumn("dbo.Students", "GID", c => c.Int(nullable: true));
            CreateIndex("dbo.Students", "GID");
            AddForeignKey("dbo.Students", "GID", "dbo.Groups", "GID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "GID", "dbo.Groups");
            DropIndex("dbo.Students", new[] { "GID" });
            AlterColumn("dbo.Students", "GID", c => c.Int());
            RenameColumn(table: "dbo.Students", name: "GID", newName: "group_GID");
            CreateIndex("dbo.Students", "group_GID");
            AddForeignKey("dbo.Students", "group_GID", "dbo.Groups", "GID");
        }
    }
}
