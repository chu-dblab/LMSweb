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
        }
    }
}
