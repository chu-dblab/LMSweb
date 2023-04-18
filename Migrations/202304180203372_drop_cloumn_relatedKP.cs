namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class drop_cloumn_relatedKP : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Missions", "relatedKP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Missions", "relatedKP", c => c.String());
        }
    }
}
