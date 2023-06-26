using LMSweb.Assets;
using System.Collections.Generic;
using System;

namespace LMSweb.ViewModel.GroupAssessment
{
    public class SelectCommentType
    {
        public LMSweb.Models.CommentType commentTypeList { get; set; }
    }

    public class CommentViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string MissionID { get; set; }
        public string MissionName { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        

        public GlobalClass.TaskSteps TaskSteps { get; set; }
        public LMSweb.Models.Comment Comment { get; set; }
        public IEnumerable<LMSweb.Models.Comment> Comments { get; set; }
        public LMSweb.Models.CommentType CommentType { get; set; }
        public IEnumerable<LMSweb.Models.CommentType> CommentTypes { get; set; }
    }

    public class FeedbackViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string MissionID { get; set; }
        public string MissionName { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }

        public GlobalClass.TaskSteps TaskSteps { get; set; }

        public LMSweb.Models.Comment Comment { get; set; }
        public IEnumerable<LMSweb.Models.Comment> Comments { get; set; }

        public LMSweb.Models.Comment Feedback { get; set; }
        public IEnumerable<LMSweb.Models.Comment> Feedbacks { get; set; }

        public LMSweb.Models.CommentType FeedbackType { get; set; }
        public IEnumerable<LMSweb.Models.CommentType> FeedbackTypes { get; set; }
    }
}