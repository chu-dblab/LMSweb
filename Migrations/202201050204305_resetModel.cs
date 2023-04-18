namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resetModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CID = c.String(nullable: false, maxLength: 128),
                        CName = c.String(nullable: false),
                        TID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CID)
                .ForeignKey("dbo.Teachers", t => t.TID, cascadeDelete: true)
                .Index(t => t.TID);
            
            CreateTable(
                "dbo.KnowledgePoints",
                c => new
                    {
                        KID = c.Int(nullable: false, identity: true),
                        CID = c.String(nullable: false, maxLength: 128),
                        KContent = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.KID)
                .ForeignKey("dbo.Courses", t => t.CID, cascadeDelete: true)
                .Index(t => t.CID);
            
            CreateTable(
                "dbo.LearningBehaviors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ActionType = c.String(nullable: false),
                        subAction = c.String(nullable: false),
                        Detail = c.String(nullable: false),
                        Time = c.String(),
                        CID = c.String(maxLength: 128),
                        StudentMissions_SID = c.String(maxLength: 128),
                        StudentMissions_MID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CID)
                .ForeignKey("dbo.StudentMissions", t => new { t.StudentMissions_SID, t.StudentMissions_MID })
                .Index(t => t.CID)
                .Index(t => new { t.StudentMissions_SID, t.StudentMissions_MID });
            
            CreateTable(
                "dbo.StudentMissions",
                c => new
                    {
                        SID = c.String(nullable: false, maxLength: 128),
                        MID = c.String(nullable: false, maxLength: 128),
                        total_score = c.Int(nullable: false),
                        selfAssessment_SEID = c.Int(),
                    })
                .PrimaryKey(t => new { t.SID, t.MID })
                .ForeignKey("dbo.SelfAssessments", t => t.selfAssessment_SEID)
                .Index(t => t.selfAssessment_SEID);
            
            CreateTable(
                "dbo.PeerAssessments",
                c => new
                    {
                        PEID = c.Int(nullable: false, identity: true),
                        PeerA = c.String(nullable: false),
                        AssessedSID = c.String(nullable: false),
                        StudentMissions_SID = c.String(maxLength: 128),
                        StudentMissions_MID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PEID)
                .ForeignKey("dbo.StudentMissions", t => new { t.StudentMissions_SID, t.StudentMissions_MID })
                .Index(t => new { t.StudentMissions_SID, t.StudentMissions_MID });
            
            CreateTable(
                "dbo.SelfAssessments",
                c => new
                    {
                        SEID = c.Int(nullable: false, identity: true),
                        CooperationLevel = c.Int(nullable: false),
                        PersonalContributionLevel = c.Int(nullable: false),
                        SelfA = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SEID);
            
            CreateTable(
                "dbo.Missions",
                c => new
                    {
                        MID = c.String(nullable: false, maxLength: 128),
                        Start = c.String(nullable: false),
                        End = c.String(nullable: false),
                        MName = c.String(nullable: false),
                        Tip = c.String(),
                        MDetail = c.String(nullable: false),
                        AddMetacognition = c.Boolean(nullable: false),
                        discuss_k = c.Int(nullable: false),
                        chart_k = c.Int(nullable: false),
                        code_k = c.Int(nullable: false),
                        eva_k = c.Int(nullable: false),
                        per_k = c.Int(nullable: false),
                        CID = c.String(maxLength: 128),
                        Group_GID = c.Int(),
                    })
                .PrimaryKey(t => t.MID)
                .ForeignKey("dbo.Courses", t => t.CID)
                .ForeignKey("dbo.Groups", t => t.Group_GID)
                .Index(t => t.CID)
                .Index(t => t.Group_GID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        SID = c.String(nullable: false, maxLength: 128),
                        SName = c.String(nullable: false),
                        Grade = c.String(nullable: false),
                        SPassword = c.String(nullable: false, maxLength: 18),
                        Sex = c.String(nullable: false),
                        Stage = c.String(nullable: false),
                        Score = c.String(),
                        CID = c.String(nullable: false, maxLength: 128),
                        GID = c.String(),
                        group_GID = c.Int(),
                        Mission_MID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SID)
                .ForeignKey("dbo.Courses", t => t.CID, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.group_GID)
                .ForeignKey("dbo.Missions", t => t.Mission_MID)
                .Index(t => t.CID)
                .Index(t => t.group_GID)
                .Index(t => t.Mission_MID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GID = c.Int(nullable: false, identity: true),
                        GName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GID);
            
            CreateTable(
                "dbo.TeacherAssessments",
                c => new
                    {
                        TEID = c.Int(nullable: false, identity: true),
                        TeacherA = c.String(nullable: false),
                        GroupAchievementLevel = c.Int(nullable: false),
                        GID = c.String(),
                        group_GID = c.Int(),
                    })
                .PrimaryKey(t => t.TEID)
                .ForeignKey("dbo.Groups", t => t.group_GID)
                .Index(t => t.group_GID);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        TID = c.String(nullable: false, maxLength: 128),
                        TName = c.String(nullable: false),
                        TPassword = c.String(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.TID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "TID", "dbo.Teachers");
            DropForeignKey("dbo.Students", "Mission_MID", "dbo.Missions");
            DropForeignKey("dbo.TeacherAssessments", "group_GID", "dbo.Groups");
            DropForeignKey("dbo.Students", "group_GID", "dbo.Groups");
            DropForeignKey("dbo.Missions", "Group_GID", "dbo.Groups");
            DropForeignKey("dbo.Students", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.Missions", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.StudentMissions", "selfAssessment_SEID", "dbo.SelfAssessments");
            DropForeignKey("dbo.PeerAssessments", new[] { "StudentMissions_SID", "StudentMissions_MID" }, "dbo.StudentMissions");
            DropForeignKey("dbo.LearningBehaviors", new[] { "StudentMissions_SID", "StudentMissions_MID" }, "dbo.StudentMissions");
            DropForeignKey("dbo.LearningBehaviors", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.KnowledgePoints", "CourseID", "dbo.Courses");
            DropIndex("dbo.TeacherAssessments", new[] { "group_GID" });
            DropIndex("dbo.Students", new[] { "Mission_MID" });
            DropIndex("dbo.Students", new[] { "group_GID" });
            DropIndex("dbo.Students", new[] { "CourseID" });
            DropIndex("dbo.Missions", new[] { "Group_GID" });
            DropIndex("dbo.Missions", new[] { "CourseID" });
            DropIndex("dbo.PeerAssessments", new[] { "StudentMissions_SID", "StudentMissions_MID" });
            DropIndex("dbo.StudentMissions", new[] { "selfAssessment_SEID" });
            DropIndex("dbo.LearningBehaviors", new[] { "StudentMissions_SID", "StudentMissions_MID" });
            DropIndex("dbo.LearningBehaviors", new[] { "CourseID" });
            DropIndex("dbo.KnowledgePoints", new[] { "CourseID" });
            DropIndex("dbo.Courses", new[] { "TID" });
            DropTable("dbo.Teachers");
            DropTable("dbo.TeacherAssessments");
            DropTable("dbo.Groups");
            DropTable("dbo.Students");
            DropTable("dbo.Missions");
            DropTable("dbo.SelfAssessments");
            DropTable("dbo.PeerAssessments");
            DropTable("dbo.StudentMissions");
            DropTable("dbo.LearningBehaviors");
            DropTable("dbo.KnowledgePoints");
            DropTable("dbo.Courses");
        }
    }
}
