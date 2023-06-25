namespace LMSweb.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LMSweb.Models.LMSmodel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LMSweb.Models.LMSmodel context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.ExperimentalProcedures.AddOrUpdate(
                p => p.EProcedureID,
                new Models.ExperimentalProcedure { EProcedureID = "1", Name = "目標設置" },
                new Models.ExperimentalProcedure { EProcedureID = "2", Name = "自我反思" },
                new Models.ExperimentalProcedure { EProcedureID = "3", Name = "團隊反思" },
                new Models.ExperimentalProcedure { EProcedureID = "4", Name = "學習監督" },
                new Models.ExperimentalProcedure { EProcedureID = "5", Name = "同儕互評" },
                new Models.ExperimentalProcedure { EProcedureID = "6", Name = "評價回饋" }
                );

            context.CommentTypes.AddOrUpdate(
                p => p.CommentTypeID,
                new Models.CommentType { CommentTypeID = "01", CTContent = "我們想稱讚這組的流程圖或程式碼", CTAttribute = "評語" },
                new Models.CommentType { CommentTypeID = "02", CTContent = "我們想批評這組的流程圖或程式碼", CTAttribute = "評語" },
                new Models.CommentType { CommentTypeID = "03", CTContent = "我們想指出這組有錯誤的地方", CTAttribute = "評語" },
                new Models.CommentType { CommentTypeID = "04", CTContent = "我們想請這組想想看如何改進他們的流程圖或程式碼", CTAttribute = "評語" },
                new Models.CommentType { CommentTypeID = "05", CTContent = "我們想說跟這組的作品無關的評論", CTAttribute = "評語" },
                new Models.CommentType { CommentTypeID = "06", CTContent = "我們想稱讚這組給的評語", CTAttribute = "回饋" },
                new Models.CommentType { CommentTypeID = "07", CTContent = "我們想批評這組給的評語", CTAttribute = "回饋" },
                new Models.CommentType { CommentTypeID = "08", CTContent = "我們想指出這組給的評語有錯誤的地方", CTAttribute = "回饋" },
                new Models.CommentType { CommentTypeID = "09", CTContent = "我們想請這組想想看如何改進他們的評語", CTAttribute = "回饋" },
                new Models.CommentType { CommentTypeID = "10", CTContent = "我們想說跟這組的評語無關的回饋", CTAttribute = "回饋" }
            );
        }
    }
}
