using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LMSweb.Models;

namespace LMSweb.ViewModel
{
    public class EvalutionViewModel
    {
        public string CID { get; set; }
        public string CName { get; set; }
        public string MID { get; set; }
        public string MName { get; set; }
        public string SID { get; set; }
        public string SName { get; set; }
        public int GID { get; set; }
        public string GName { get; set; }
        public string EvaluatorSID { get; set; }
        public string DrawingImgPath { get; set; }
        public string CodeText { get; set; }
        public string CodePath { get; set; }
        public bool IsCodeImg { get; set; }
        public bool IsDiscuss { get; set; }
        public string TID { get; set; }
        public IEnumerable<LMSweb.Models.DefaultQuestion> DefaultQuestion { get; set; }
        public IEnumerable<LMSweb.Models.GroupQuestion> GroupQuestion { get; set; }
        public List<Evalution_Response> ERs { get; set; }
        public List<Group_Response> GRs { get; set; }
        public List<Teacher_Response> TRs { get; set; }

        public LMSweb.Models.GroupER GroupER { get; set; }
        public LMSweb.Models.EvalutionResponse Response { get; set; }
        public LMSweb.Models.GroupOption GroupOption { get; set; }

        public List<LMSweb.Models.StudentCode> IsUploadCode { get; set; }
        public List<LMSweb.Models.StudentDraw> IsUploadDraw { get; set; }
        public List<LMSweb.Models.Group> Groups { get; set; }
        public List<LMSweb.Models.Course> Courses { get; set; }
        public LMSweb.Models.TeacherAssessment TeacherAssessment { get; set; }
        public LMSweb.Models.PeerAssessment PeerAssessment { get; set; }
        public LMSweb.Models.Group Group { get; set; }
        public LMSweb.Models.Course Course { get; set; }
        public LMSweb.Models.Student Student { get; set; }
        public List<LMSweb.Models.Student> Students { get; set; }
        public List<LMSweb.Models.PeerAssessment> PeerAssessments { get; set; }
        public List<LMSweb.Models.GroupER> Aswer { get; set; }
        public List<LMSweb.Models.EvalutionResponse> SelfPeerAswer { get; set; }
    }
    public class Evalution_Response
    {
        public int qid { get; set; }


        [Required]
        public string response { get; set; }
        public string comments { get; set; }
        public string sid { get; set; }
        public string mid { get; set; }

    }

    public class Group_Response
    {
        public int qid { get; set; }


        [Required]
        public string response { get; set; }
        public string comments { get; set; }
        public string gid { get; set; }

    }
    public class Teacher_Response
    {
        public int qid { get; set; }


        [Required]
        public string response { get; set; }
        public string comments { get; set; }
        public string tid { get; set; }

    }
}