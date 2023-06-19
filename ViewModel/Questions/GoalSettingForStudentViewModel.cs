using LMSweb.Assets;
using LMSweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel.Questions
{
    public class GoalSettingForStudentViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string MissionID { get; set; }
        public string MissionName { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }

        public GlobalClass.TaskSteps TaskSteps { get; set; }

        public List<Question_Response> QuestionResponses { get; set; }

        public List<DefaultQuestion> DefaultQuestions { get; set; }
    }
    public class Question_Response
    {
        public int qid { get; set; }

        public string response { get; set; }

        public string mid { get; set; }
    }
}