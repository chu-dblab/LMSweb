using System;
using System.Data.Entity;
using System.Linq;

namespace LMSweb.Models
{
    public class LMSmodel : DbContext
    {
        public LMSmodel(): base("name=LMSmodel")
        {
        }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<KnowledgePoint> KnowledgePoints { get; set; }
        public DbSet<LearningBehavior> LearnB { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<PeerAssessment> PeerA { get; set; }
        public DbSet<EvalutionResponse> EvalutionResponse { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentMission> StudentMissions { get; set; }
        public DbSet<TeacherAssessment> TeacherA { get; set; }
        public DbSet<StudentCode> StudentCodes { get; set; }
        public DbSet<StudentDraw> StudentDraws { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set;}
        public DbSet<DefaultQuestion> DefaultQuestions { get; set; }
        public DbSet<DefaultOption> DefaultOptions { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<GroupQuestion> GroupQuestions { get; set; }
        public DbSet<GroupOption> GroupOptions { get; set; }
        public DbSet<GroupER> GroupERs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Execution> Executions { get; set; }
        public DbSet<ExperimentalProcedure> ExperimentalProcedures { get; set; }
        public DbSet<QuestionNew> QuestionNews { get; set; }
        public DbSet<OptionNew> OptionNews { get; set; }
        public DbSet<CommentType> CommentTypes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Provided> Provideds { get; set; }
    }

}