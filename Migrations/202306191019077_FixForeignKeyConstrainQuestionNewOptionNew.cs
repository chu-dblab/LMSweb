namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixForeignKeyConstrainQuestionNewOptionNew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OptionNews",
                c => new
                    {
                        OptionID = c.Int(nullable: false, identity: true),
                        OContent = c.String(),
                        QuestionNewID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OptionID)
                .ForeignKey("dbo.QuestionNews", t => t.QuestionNewID, cascadeDelete: true)
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
