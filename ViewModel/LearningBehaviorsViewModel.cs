using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class LearningBehaviorsViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        
        public List<MissionLearningBehaviorsViewModel> MissionLearningBehaviors { get; set; }
        public List<GroupLearningBehaviorsViewModel> GroupLearningBehaviors { get; set; }

        public List<Chart.BarChartViewModel> ChartsData { get; set; }
    }

    public class GroupLearningBehaviorsViewModel
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }

        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public bool IsLeader { get; set; }
    }

    public class MissionLearningBehaviorsViewModel
    {
        public string MisstionID { get; set; }
        public string MisstionName { get; set; }
    }
}