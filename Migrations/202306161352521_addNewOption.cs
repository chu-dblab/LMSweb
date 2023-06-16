namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewOption : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OptionNews",
                c => new
                    {
                        OptionID = c.Int(nullable: false, identity: true),
                        OContent = c.String(),
                        QuestionNewID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OptionID)
                .ForeignKey("dbo.QuestionNews", t => t.QuestionNewID)
                .Index(t => t.QuestionNewID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OptionNews", "QuestionNewID", "dbo.QuestionNews");
            DropIndex("dbo.OptionNews", new[] { "QuestionNewID" });
            DropTable("dbo.OptionNews");
        }
    }
}
