using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMSweb.Models;
using LMSweb.ViewModel;

namespace LMSweb.Controllers
{
    public class QuestionsController : Controller
    {
        private LMSmodel db = new LMSmodel();

        public ActionResult SelectQuestion(string mid, string cid)
        {
            SurveyQuestionViewModel sQvmodel = new SurveyQuestionViewModel();
            sQvmodel.DefaultQuestions = db.DefaultQuestions.Where(dq => dq.Class == "目標設置").Include(dq => dq.DefaultOptions).ToList();
            sQvmodel.MID = mid;
            sQvmodel.CID = cid;
            sQvmodel.CName = db.Courses.Find(cid).CName;

            return View(sQvmodel);
        }
        public ActionResult SelectReflectionQuestion(string mid, string cid)
        {
            SurveyQuestionViewModel sQvmodel = new SurveyQuestionViewModel();
            sQvmodel.DefaultQuestions = db.DefaultQuestions.Where(dq => dq.Class == "自我反思").Include(dq => dq.DefaultOptions).ToList();
            sQvmodel.MID = mid;
            sQvmodel.CID = cid;
            sQvmodel.CName = db.Courses.Find(cid).CName;

            return View(sQvmodel);
        }

        public ActionResult AddQuestion(string mid, string cid,int dqid)
        {
            DefaultQuestion defaultQuestion = db.DefaultQuestions.Find(dqid); //default
            var question = new Question();

            question.Description = defaultQuestion.Description;
            question.MID = mid;
            question.Class = "目標設置";
            question.Type = defaultQuestion.Type;

            db.Questions.Add(question);
            db.SaveChanges();
            
            if(! (defaultQuestion.Type == "問答"))
            {
                var defaultOptions = db.DefaultOptions.Where(o => o.DQID == dqid).ToList();
                foreach(var d_option in defaultOptions)
                {
                    var option = new Option();
                    option.QID = question.QID;
                    option.Question = question;
                    option.OptionName = d_option.DOptions;
                    db.Options.Add(option);
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { cid = cid, mid = mid });
        }

        public ActionResult AddReflectionQuestion(string mid, string cid, int dqid)
        {
            DefaultQuestion defaultQuestion = db.DefaultQuestions.Find(dqid); //default
            var question = new Question();
            question.Description = defaultQuestion.Description;
            question.MID = mid;
            question.Class = "自我反思";
            question.Type = defaultQuestion.Type;

            db.Questions.Add(question);
            db.SaveChanges();

            if (!(defaultQuestion.Type == "問答"))
            {
                var defaultOptions = db.DefaultOptions.Where(o => o.DQID == dqid).ToList();
                foreach (var d_option in defaultOptions)
                {
                    var option = new Option();
                    option.QID = question.QID;
                    option.Question = question;
                    option.OptionName = d_option.DOptions;
                    db.Options.Add(option);
                }
                db.SaveChanges();
            }

            return RedirectToAction("ReflectionIndex", new { cid = cid, mid = mid });
        }

        public ActionResult Index(SurveyViewModel svmodel, string mid, string cid)
        {
            var questions = db.Questions.Where(q => q.MID == mid && q.Class =="目標設置").Include(q => q.Options);
            svmodel.Questions = questions;
            svmodel.CID = cid;
            svmodel.MID = mid;
            svmodel.CName = db.Courses.Find(cid).CName;

            return View(svmodel);
        }
        public ActionResult ReflectionIndex(SurveyViewModel svmodel, string mid, string cid)
        {
            var questions = db.Questions.Where(q => q.MID == mid && q.Class == "自我反思").Include(q => q.Options);
            svmodel.Questions = questions;
            svmodel.CID = cid;
            svmodel.MID = mid;
            svmodel.CName = db.Courses.Find(cid).CName;

            return View(svmodel);
        }

        public ActionResult Create(string mid, string cid)
        {
            SurveyQuestionViewModel suQvmodel = new SurveyQuestionViewModel();
            suQvmodel.Question = new Question();
            suQvmodel.Question.MID = mid;
            suQvmodel.MID = mid;
            suQvmodel.CID = cid;
            suQvmodel.CName = db.Courses.Find(cid).CName;

            return View(suQvmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SurveyQuestionViewModel suQvmodel)
        {
            if (ModelState.IsValid)
            {
                var mission = db.Missions.Find(suQvmodel.Question.MID);
                mission.Questions.Add(suQvmodel.Question);
                db.Questions.Add(suQvmodel.Question);
                db.SaveChanges();

                if (!(suQvmodel.Question.Type == "問答"))
                {
                    foreach (var s_option in suQvmodel.String_Options)
                    {
                        if (s_option != "")
                        {
                            var option = new Option();
                            option.QID = suQvmodel.Question.QID;
                            option.OptionName = s_option;
                            option.Question = suQvmodel.Question;

                            db.Options.Add(option);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index", new { cid = suQvmodel.CID, mid = suQvmodel.MID });
            }

            return View(suQvmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReflection(SurveyQuestionViewModel suQvmodel)
        {
            if (ModelState.IsValid)
            {
                var mission = db.Missions.Find(suQvmodel.Question.MID);
                mission.Questions.Add(suQvmodel.Question);

                db.Questions.Add(suQvmodel.Question);
                db.SaveChanges();

                if (!(suQvmodel.Question.Type == "問答"))
                {
                    foreach (var s_option in suQvmodel.String_Options)
                    {
                        if (s_option != "")
                        {
                            var option = new Option();
                            option.QID = suQvmodel.Question.QID;
                            option.OptionName = s_option;
                            option.Question = suQvmodel.Question;

                            db.Options.Add(option);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("ReflectionIndex", new { cid = suQvmodel.CID, mid = suQvmodel.MID });
            }

            return View(suQvmodel);
        }
        public ActionResult CreateReflection(string mid, string cid)
        {
            SurveyQuestionViewModel suQvmodel = new SurveyQuestionViewModel();
            suQvmodel.Question = new Question();
            suQvmodel.Question.MID = mid;
            suQvmodel.CID = cid;
            suQvmodel.MID = mid;
            suQvmodel.CName = db.Courses.Find(cid).CName;

            return View(suQvmodel);
        }

        public ActionResult Edit(int qid, string mid, string cid)
        {
            Question question = db.Questions.Where(qus => qus.QID == qid).Include(qus => qus.Options).Single();
            QuestionViewModel Qvmodel = new QuestionViewModel();
            if (question == null)
            {
                return HttpNotFound();
            }

            Qvmodel.Question = question;
            Qvmodel.CID = cid;
            Qvmodel.CName = db.Courses.Find(cid).CName;

            return View(Qvmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionViewModel Qvmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Qvmodel.Question).State = EntityState.Modified;
                var question_et = db.Questions.Where(qus => qus.QID == Qvmodel.Question.QID).Include(qus => qus.Options).Single();
                if (question_et.Options != null)
                {
                    db.Options.RemoveRange(question_et.Options);
                    question_et.Options.Clear();
                }
                Qvmodel.Question.mission = question_et.mission;
                if (Qvmodel.Question.Type != "問答")
                {
                    foreach (var s_option in Qvmodel.String_Options)
                    {
                        if (s_option != "")
                        {
                            var option = new Option();
                            option.QID = Qvmodel.Question.QID;
                            option.OptionName = s_option;
                            option.Question = Qvmodel.Question;
                            db.Options.Add(option);
                            Qvmodel.Question.Options.Add(option);
                        }
                    }
                }
                db.SaveChanges();

                return RedirectToAction("Index", new { cid = Qvmodel.CID, mid = question_et.MID });
            }

           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int qidHidden, string midHidden, string cidHidden)
        {
            Question question = db.Questions.Find(qidHidden);
            db.Questions.Remove(question);

            var option = db.Options.Where(o => o.QID == qidHidden);
            db.Options.RemoveRange(option);

            db.SaveChanges();

            return RedirectToAction("Index", new { cid = cidHidden, mid = midHidden });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //以下自互評頁面
        public ActionResult PersonalAbility(SurveyViewModel svmodel, string mid, string cid)
        {
            var questions = db.Questions.Where(q => q.MID == mid && q.Class == "個人能力").Include(q => q.Options);
            svmodel.Questions = questions;
            svmodel.CID = cid;
            svmodel.MID = mid;
            svmodel.CName = db.Courses.Find(cid).CName;

            return View(svmodel);
        }
        public ActionResult TeamworkAbility(SurveyViewModel svmodel, string mid, string cid)
        {
            var questions = db.Questions.Where(q => q.MID == mid && q.Class == "合作能力").Include(q => q.Options);
            svmodel.Questions = questions;
            svmodel.CID = cid;
            svmodel.MID = mid;
            svmodel.CName = db.Courses.Find(cid).CName;

            return View(svmodel);
        }

        public ActionResult SelectPersonalE(string mid, string cid)
        {
            SurveyQuestionViewModel sQvmodel = new SurveyQuestionViewModel();
            sQvmodel.DefaultQuestions = db.DefaultQuestions.Where(dq => dq.Class == "個人能力").Include(dq => dq.DefaultOptions).ToList();
            sQvmodel.MID = mid;
            sQvmodel.CID = cid;
            sQvmodel.CName = db.Courses.Find(cid).CName;

            return View(sQvmodel);
        }
        public ActionResult SelectTeamworkE(string mid, string cid)
        {
            SurveyQuestionViewModel sQvmodel = new SurveyQuestionViewModel();
            sQvmodel.DefaultQuestions = db.DefaultQuestions.Where(dq => dq.Class == "合作能力").Include(dq => dq.DefaultOptions).ToList();
            sQvmodel.MID = mid;
            sQvmodel.CID = cid;
            sQvmodel.CName = db.Courses.Find(cid).CName;

            return View(sQvmodel);
        }

        public ActionResult AddPersonalE(string mid, string cid, int dqid)
        {
            DefaultQuestion defaultQuestion = db.DefaultQuestions.Find(dqid); //default
            var question = new Question();
            question.Description = defaultQuestion.Description;
            question.MID = mid;
            question.Class = "個人能力";
            question.Type = defaultQuestion.Type;

            db.Questions.Add(question);
            db.SaveChanges();

            if (!(defaultQuestion.Type == "問答"))
            {
                var defaultOptions = db.DefaultOptions.Where(o => o.DQID == dqid).ToList();
                foreach (var d_option in defaultOptions)
                {
                    var option = new Option();
                    option.QID = question.QID;
                    option.Question = question;
                    option.OptionName = d_option.DOptions;
                    db.Options.Add(option);
                }
                db.SaveChanges();
            }

            return RedirectToAction("PersonalAbility", new { cid = cid, mid = mid });
        }
        public ActionResult AddTeamworkE(string mid, string cid, int dqid)
        {
            DefaultQuestion defaultQuestion = db.DefaultQuestions.Find(dqid); //default
            var question = new Question();
            question.Description = defaultQuestion.Description;
            question.MID = mid;
            question.Class = "合作能力";
            question.Type = defaultQuestion.Type;

            db.Questions.Add(question);
            db.SaveChanges();

            if (!(defaultQuestion.Type == "問答"))
            {
                var defaultOptions = db.DefaultOptions.Where(o => o.DQID == dqid).ToList();
                foreach (var d_option in defaultOptions)
                {
                    var option = new Option();
                    option.QID = question.QID;
                    option.Question = question;
                    option.OptionName = d_option.DOptions;

                    db.Options.Add(option);
                }
                db.SaveChanges();
            }

            return RedirectToAction("TeamworkAbility", new { cid = cid, mid = mid });
        }
        public ActionResult PersonalECreate(string mid, string cid)
        {
            SurveyQuestionViewModel suQvmodel = new SurveyQuestionViewModel();
            suQvmodel.Question = new Question();
            suQvmodel.Question.MID = mid;
            suQvmodel.MID = mid;
            suQvmodel.CID = cid;
            suQvmodel.CName = db.Courses.Find(cid).CName;

            return View(suQvmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalECreate(SurveyQuestionViewModel suQvmodel)
        {
            if (ModelState.IsValid)
            {
                var mission = db.Missions.Find(suQvmodel.Question.MID);
                mission.Questions.Add(suQvmodel.Question);
                db.Questions.Add(suQvmodel.Question);
                db.SaveChanges();

                if (!(suQvmodel.Question.Type == "問答"))
                {
                    foreach (var s_option in suQvmodel.String_Options)
                    {
                        if (s_option != "")
                        {
                            var option = new Option();
                            option.QID = suQvmodel.Question.QID;
                            option.OptionName = s_option;
                            option.Question = suQvmodel.Question;

                            db.Options.Add(option);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("PersonalAbility", new { cid = suQvmodel.CID, mid = suQvmodel.MID });
            }

            return View(suQvmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamworkECreate(SurveyQuestionViewModel suQvmodel)
        {
            if (ModelState.IsValid)
            {
                var mission = db.Missions.Find(suQvmodel.Question.MID);
                mission.Questions.Add(suQvmodel.Question);

                db.Questions.Add(suQvmodel.Question);
                db.SaveChanges();

                if (!(suQvmodel.Question.Type == "問答"))
                {
                    foreach (var s_option in suQvmodel.String_Options)
                    {
                        if (s_option != "")
                        {
                            var option = new Option();
                            option.QID = suQvmodel.Question.QID;
                            option.OptionName = s_option;
                            option.Question = suQvmodel.Question;

                            db.Options.Add(option);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("TeamworkAbility", new { cid = suQvmodel.CID, mid = suQvmodel.MID });
            }

            return View(suQvmodel);
        }
        public ActionResult TeamworkECreate(string mid, string cid)
        {
            SurveyQuestionViewModel suQvmodel = new SurveyQuestionViewModel();
            suQvmodel.Question = new Question();
            suQvmodel.Question.MID = mid;
            suQvmodel.CID = cid;
            suQvmodel.MID = mid;
            suQvmodel.CName = db.Courses.Find(cid).CName;

            return View(suQvmodel);
        }

        public ActionResult EvalutionEdit(int qid, string mid, string cid)
        {
            Question question = db.Questions.Where(qus => qus.QID == qid).Include(qus => qus.Options).Single();
            QuestionViewModel Qvmodel = new QuestionViewModel();
            Qvmodel.Question = question;
            Qvmodel.CID = cid;
            Qvmodel.CName = db.Courses.Find(cid).CName;

            if (question == null)
            {
                return HttpNotFound();
            }

            return View(Qvmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvalutionEdit(QuestionViewModel Qvmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Qvmodel.Question).State = EntityState.Modified;
                var question_et = db.Questions.Where(qus => qus.QID == Qvmodel.Question.QID).Include(qus => qus.Options).Single();
                if (question_et.Options != null)
                {
                    db.Options.RemoveRange(question_et.Options);
                    question_et.Options.Clear();
                }
                Qvmodel.Question.mission = question_et.mission;
                if (Qvmodel.Question.Type != "問答")
                {
                    foreach (var s_option in Qvmodel.String_Options)
                    {
                        if (s_option != "")
                        {
                            var option = new Option();
                            option.QID = Qvmodel.Question.QID;
                            option.OptionName = s_option;
                            option.Question = Qvmodel.Question;

                            db.Options.Add(option);
                            Qvmodel.Question.Options.Add(option);
                        }
                    }
                }
                db.SaveChanges();

                return RedirectToAction("PersonalAbility", new { cid = Qvmodel.CID, mid = question_et.MID });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvalutionDelete(int qidHidden, string midHidden, string cidHidden)
        {

            Question question = db.Questions.Find(qidHidden);
            db.Questions.Remove(question);

            var option = db.Options.Where(o => o.QID == qidHidden);
            db.Options.RemoveRange(option);

            db.SaveChanges();

            return RedirectToAction("PersonalAbility", new { cid = cidHidden, mid = midHidden });
        }

        public ActionResult ManagementInterface()
        {


            return View();
        }
    }

    

}
