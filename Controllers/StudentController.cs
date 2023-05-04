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
using System.Security.Claims;
using System.IO;
using System.Web.Configuration;
using LMSweb.Service;
using System.Text.RegularExpressions;

namespace LMSweb.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSmodel db = new LMSmodel();

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Home()
        {
            StudentHomePageViewModel vmodel = new StudentHomePageViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "UID").FirstOrDefault();   //抓出當初記載Claims陣列中的SID
            var sid = claimData.Value; //取值(因為只有一筆)

            //var data = from c in db.Courses
            //           from s in db.Students
            //           from g in db.Groups
            //           from t in db.Teachers
            //           where s.CID == c.CID && c.TID == t.TID && s.GID == g.GID&& s.SID == sid
            //           select new StudentHomeViewModel
            //           {
            //               CourseID = c.CID,
            //               CourseName = c.CName,
            //               TeacherName = t.TName,
            //               GroupName = g.GName
            //           };
            //return View();


            var stuG = db.Students.Find(sid).Group;
            var studentCourse = db.Students.Where(s => s.SID == sid);
            var stuCourse = db.Students.Find(sid);
            var cid = stuCourse.CID;
            var course = db.Courses.Find(cid);
            var cname = course.CName;
            var tname = course.Teacher.TName;

            vmodel.CID = cid;
            vmodel.CName = cname;
            vmodel.TName = tname;
            if (stuG == null)
            {
                vmodel.Enter = false;
                ModelState.AddModelError("", "目前還沒有分組，請告知授課老師");

                return View(vmodel);
            }
            else
            {
                vmodel.Enter = true;
                vmodel.Groups = db.Groups.Where(g => g.GID == stuG.GID).ToList();
                vmodel.GName = stuG.GName;

                return View(vmodel);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentCourse(string cid)
        {
            CourseViewModel model = new CourseViewModel();
            if (cid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(cid);
            if (course == null)
            {
                return HttpNotFound();
            }

            var cname = course.CName;
            model.CID = cid;
            model.CName = cname;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentMission(string cid)
        {
            MissionViewModel model = new MissionViewModel();
            var cname = db.Courses.Find(cid).CName;
            model.missions = db.Missions.Where(m => m.CID == cid);
            model.CID = cid;
            model.CName = cname;

            if (cid == null)
            {
                model.missions = db.Missions.Where(m => m.CID == cid);
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentMissionDetail(string cid, string mid)
        {
            if (mid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission mission = db.Missions.Find(mid);
            if (mission == null)
            {
                return HttpNotFound();
            }
            var model = new MissionViewModel();
            //var kps = mission.relatedKP.Split(',');
            var course = db.Courses.Find(cid);
            var mname = db.Missions.Find(mid).MName;
            model.CID = cid;
            model.MID = mid;
            model.mis = mission;
            model.CName = course.CName;
            model.course = course;
            model.MName = mname;
            model.KContents = new List<string>();
            /*for (int i = 0; i < kps.Length - 1; i++)
            {
                model.KContents.Add(db.KnowledgePoints.Find(int.Parse(kps[i])).KContent);
            }*/

            return View(model);
        }

        public ActionResult StudentMDetailWindow(string cid, string mid)
        {
            if (mid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission mission = db.Missions.Find(mid);
            if (mission == null)
            {
                return HttpNotFound();
            }
            var model = new MissionViewModel();
            var CID = db.Missions.Find(mid).CID;
            var cname = db.Courses.Find(CID).CName;
            //var kps = mission.relatedKP.Split(',');
            model.mis = mission;
            model.CID = CID;
            model.MID = mid;
            model.CName = cname;
            model.MName = db.Missions.Find(mid).MName;
            model.KContents = new List<string>();
            /*for (int i = 0; i < kps.Length - 1; i++)
            {
                model.KContents.Add(db.KnowledgePoints.Find(int.Parse(kps[i])).KContent);
            }*/

            return View(model);
        }

        private string codefileSavedPath = WebConfigurationManager.AppSettings["CodePath"];

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentCoding(string mid, string cid)
        {
            StudentCodingViewModel model = new StudentCodingViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();   //抓出當初記載Claims陣列中的SID
            var sid = claimData[0].Value;
            var group = db.Students.Find(sid).Group;
            var gid = group.GID;
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            var readcode = new TextIO();
            var code = db.StudentCodes.Find(cid, mid, gid);
            var pt = db.StudentCodes.Where(p => p.GID == gid && p.MID == mid).FirstOrDefault();
            model.CID = cid;
            model.MID = mid;
            model.MName = mname;
            model.CName = cname;
            model.GID = gid;
            model.IsDiscuss = misChat;
            model.End = db.Missions.Find(mid).End;

            if (code != null)
            {
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string readcodepath = $"{filePath}{code.CodePath}.txt";
                model.CodeText = readcode.readCodeText(readcodepath);
            }
            if (pt != null)
            {
                model.CodePath = pt.CodePath;
            }
            else
            {
                model.CodePath = null;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult StudentCoding(HttpPostedFileBase file, string cid, string mid)
        {
            StudentCodingViewModel model = new StudentCodingViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();   //抓出當初記載Claims陣列中的SID
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).Group;
            var gid = stuG.GID;
            var gname = stuG.GName;
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var sd = db.StudentCodes.Where(s => s.GID == gid && s.MID == mid).SingleOrDefault();

            model.GID = gid;
            model.GName = gname;
            model.CID = cid;
            model.MID = mid;
            model.MName = mname;
            model.CName = cname;

            if (sd != null)
            {
                db.StudentCodes.Remove(sd);
            }

            if (file != null && file.ContentLength > 0)
            {
                StudentCode studentCode = new StudentCode();
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string newFileName = string.Concat(
                    (model.GName + "-" + model.MID + DateTime.Now.ToString("yyMMddHHmmss")),
                    Path.GetExtension(file.FileName).ToLower());

                string fullFilePath = Path.Combine(Server.MapPath(codefileSavedPath), newFileName);
                file.SaveAs(fullFilePath);
               
                studentCode.CodePath = newFileName;
                studentCode.CID = cid;
                studentCode.MID = mid;
                studentCode.GID = gid;

                db.StudentCodes.Add(studentCode);
                db.SaveChanges();
            }

            return RedirectToAction("StudentCoding", "Student", new { mid, cid });
        }

        private string imgfileSavedPath = WebConfigurationManager.AppSettings["ImagesPath"];

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentDrawing(string mid, string cid)
        {
            DrawingViewModel model = new DrawingViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList(); //抓出當初記載Claims陣列中的SID
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).Group;
            var gid = stuG.GID;
            var gname = stuG.GName;
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).FirstOrDefault();
            var misChat = db.Missions.Find(mid).IsDiscuss;
            model.IsDiscuss = misChat;
            model.CID = cid;
            model.MID = mid;
            model.GID = gid;
            model.GName = gname;
            model.CName = cname;
            model.MName = mname;
            model.End = db.Missions.Find(mid).End;
            if (pt != null)
            {
                model.DrawingImgPath = pt.DrawingImgPath;
            }
            else
            {
                model.DrawingImgPath = null;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult StudentDrawing(HttpPostedFileBase file, string cid, string mid)
        {
            DrawingViewModel model = new DrawingViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();   //抓出當初記載Claims陣列中的SID
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).Group;
            var gid = stuG.GID;
            var gname = stuG.GName;
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var sd = db.StudentDraws.Where(s => s.GID == gid && s.MID == mid).SingleOrDefault();
            model.GID = gid;
            model.GName = gname;
            model.CID = cid;
            model.MID = mid;
            model.MName = mname;
            model.CName = cname;
            
            if(sd != null)
            {
                db.StudentDraws.Remove(sd);
            }
            if (file != null && file.ContentLength > 0)
            {
                StudentDraw studentDraw = new StudentDraw();
                string virtualBaseFilePath = Url.Content(imgfileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string newFileName = string.Concat(
                    (model.GName + "-" + model.MID + DateTime.Now.ToString("yyMMddHHmmss")),
                    Path.GetExtension(file.FileName).ToLower());

                string fullFilePath = Path.Combine(Server.MapPath(imgfileSavedPath), newFileName);
                file.SaveAs(fullFilePath);
                studentDraw.DrawingImgPath = newFileName;
                studentDraw.CID = cid;
                studentDraw.MID = mid;
                studentDraw.GID = gid;

                db.StudentDraws.Add(studentDraw);
                db.SaveChanges();
            }

            return RedirectToAction("StudentDrawing", "Student", new { mid, cid});
        }
        public ActionResult ShowImage(string id)
        {
            var dir = Server.MapPath("/Images");
            var path = Path.Combine(dir, id + ".jpg");

            return base.File(path, "image/jpg");
        }

        [Authorize(Roles = "Student")]
        public ActionResult Chat(string cid, string mid)
        {
            MissionViewModel model = new MissionViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
            var claimData = claims.Claims.Where(x => x.Type == "UID").ToList();
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).Group;
            var mission = db.Missions.Find(mid);

            model.CID = cid;
            model.MID = mid;
            model.SID = sid;
            if (stuG == null)
            {
                ModelState.AddModelError("", "目前還沒有分組，請告知授課老師");
            }
            else
            {
                var gid = stuG.GID;
                var gname = stuG.GName;
                model.GID = gid;
                model.GName = gname;
                
            }

            return View(model);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentGoalSetting(string mid, string cid)    //學生任務內容裡面的目標設置btn的Action
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var response = db.Responses.Where(r => r.SID == SID && r.MID == mid);
            var qids = response.Select(r => r.DQID ).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "目標設置").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            
            if (questions.Any())
            {
                return RedirectToAction("StudentGoal", "Student", new { cid, mid, SID });
            }
            else
            {
                GoalSettingViewModel goalSetVM = new GoalSettingViewModel();
                goalSetVM.DefaultQuestions = db.DefaultQuestions.Where(q => q.Class == "目標設置").Include(q => q.DefaultOptions);
                goalSetVM.MID = mid;
                goalSetVM.CID = cid;
                goalSetVM.SID = SID;
                goalSetVM.CName = cname;
                goalSetVM.MName = mname;
                goalSetVM.IsDiscuss = misChat;

                return View(goalSetVM);
            }

        }

        [HttpPost]
        public ActionResult StudentGoalSetting([System.Web.Http.FromBody] GoalSettingViewModel goalSetting)  //填表單送出的Post
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            
            foreach (var qr in goalSetting.QRs)
            {
                var response = new Response();
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = SID;
                response.MID = qr.mid;

                db.Responses.Add(response);
            }
            db.SaveChanges();
            
            return Json(new { redirectToUrl = Url.Action("StudentMissionDetail", "Student", new { cid = goalSetting.CID, mid = goalSetting.MID }) });
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentGoal(GoalSettingViewModel goalSetting, string cid, string mid, string SID)  ///學生已填過目標設置
        {
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            var response = db.Responses.Where(r => r.MID == mid);

            goalSetting.IsDiscuss = misChat;
            goalSetting.DefaultQuestions = db.DefaultQuestions.Where(q => q.Class == "目標設置").Include(q => q.Responses);
            goalSetting.MID = mid;
            goalSetting.CID = cid;
            goalSetting.SID = SID;
            goalSetting.CName = cname;
            goalSetting.MName = mname;

            return View(goalSetting);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentReflection(string mid, string cid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var response = db.Responses.Where(r => r.SID == SID && r.MID == mid);
            var qids = response.Select(r => r.DQID).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "自我反思").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            if (questions.Any())
            {
                return RedirectToAction("StudentReflectionResult", "Student", new { cid, mid, SID });
            }
            else
            {
                GoalSettingViewModel goalSetVM = new GoalSettingViewModel();
                goalSetVM.DefaultQuestions = db.DefaultQuestions.Where(q => q.Class == "自我反思").Include(q => q.DefaultOptions);
                goalSetVM.MID = mid;
                goalSetVM.CID = cid;
                goalSetVM.CName = cname;
                goalSetVM.MName = mname;
                goalSetVM.IsDiscuss = misChat;
                return View(goalSetVM);
            }

        }

        [HttpPost]
        public ActionResult StudentReflection([System.Web.Http.FromBody] GoalSettingViewModel goalSetting)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;

            foreach (var qr in goalSetting.QRs)
            {
                var response = new Response();
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = SID;
                response.MID = qr.mid;
                db.Responses.Add(response);
            }
            db.SaveChanges();
         
            return Json(new { redirectToUrl = Url.Action("StudentMissionDetail", "Student", new { cid = goalSetting.CID, mid = goalSetting.MID }) });
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentReflectionResult(GoalSettingViewModel goalSetting, string cid, string mid, string SID) //自我反思結果
        {
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            goalSetting.IsDiscuss = misChat;
            goalSetting.DefaultQuestions = db.DefaultQuestions.Where(q => q.Class == "自我反思").Include(q => q.Responses);
            goalSetting.MID = mid;
            goalSetting.CID = cid;
            goalSetting.SID = SID;
            goalSetting.CName = cname;
            goalSetting.MName = mname;

            return View(goalSetting);
        }
        public ActionResult StudentReflectionEdit(string cid, string mid)  //自我反思修改
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            StudentReflectionEditViewModel reflectionEditViewModel = new StudentReflectionEditViewModel();
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var response = db.Responses.Where(r => r.SID == SID && r.MID == mid);
            var qids = response.Select(r => r.DQID).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "自我反思").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;

            reflectionEditViewModel.DefaultQuestions = db.DefaultQuestions.Where(q => q.Class == "自我反思").Include(q => q.DefaultOptions);
            reflectionEditViewModel.MID = mid;
            reflectionEditViewModel.CID = cid;
            reflectionEditViewModel.SID = SID;
            reflectionEditViewModel.CName = cname;
            reflectionEditViewModel.MName = mname;
            reflectionEditViewModel.IsDiscuss = misChat;
            reflectionEditViewModel.ReflectionAswer = response.ToList();

            return View(reflectionEditViewModel);
        }

        [HttpPost]
        public ActionResult StudentReflectionEdit([System.Web.Http.FromBody] StudentReflectionEditViewModel reflectionEditViewModel, string cid, string mid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var MID = mid;
            var CID = cid;
            foreach (var qr in reflectionEditViewModel.QRs)
            {
                var response = db.Responses.Where(r => r.SID == SID && r.MID == mid && r.DQID == qr.qid).ToList()[0];
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = SID;
                response.CID = cid;
                response.MID = mid;

                db.Entry(response).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                reflectionEditViewModel.CID = cid;
              
                return Json("suc");
            }

            return View(reflectionEditViewModel);
        }

        //以下自評互評
        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentSelfEvalution(string sid, string mid, string cid)//學生任務內容裡面的目標設置btn的Action
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sname = db.Students.Find(sid).SName;
            var evalution = db.EvalutionResponse.Where(r => r.SID == SID && r.EvaluatorSID == SID && r.MID == mid);
            var qids = evalution.Select(r => r.DQID).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "自評互評").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            if (questions.Any())
            {
                return RedirectToAction("StudentSelfER", "Student", new { SID, cid, mid });
            }
            else
            {
                EvalutionViewModel SelfEVM = new EvalutionViewModel();
                SelfEVM.DefaultQuestion = db.DefaultQuestions.Where(q => q.Class == "自評互評").Include(q => q.DefaultOptions);
                SelfEVM.MID = mid;
                SelfEVM.CID = cid;
                SelfEVM.SID = SID;
                SelfEVM.EvaluatorSID = SID;
                SelfEVM.CName = cname;
                SelfEVM.MName = mname;
                SelfEVM.SName = sname;
                SelfEVM.IsDiscuss = misChat;

                return View(SelfEVM);
            }
        }

        [HttpPost]
        public ActionResult StudentSelfEvalution([System.Web.Http.FromBody] EvalutionViewModel evalution, string mid, string cid)  //填表單送出的Post
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;

            foreach (var qr in evalution.ERs)
            {
                var response = new EvalutionResponse();
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = SID;
                response.CID = cid;
                response.MID = mid;

                response.EvaluatorSID = SID;
                db.EvalutionResponse.Add(response);
            }
            db.SaveChanges();
            
            return Json(new { redirectToUrl = Url.Action("Index", "PeerAssessments", new { cid = evalution.CID, mid = evalution.MID }) });
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentSelfER(EvalutionViewModel evalution, string sid, string cid, string mid)  ///學生已填過目標設置
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sname = db.Students.Find(sid).SName;
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            evalution.IsDiscuss = misChat;
            evalution.DefaultQuestion = db.DefaultQuestions.Where( q => q.Class == "自評互評" ).Include(q => q.EvalutionResponses);
            evalution.MID = mid;
            evalution.CID = cid;
            evalution.EvaluatorSID = SID;
            evalution.CName = cname;
            evalution.SID = SID;
            evalution.MName = mname;
            evalution.SName = sname;

            return View(evalution);
        }

        public ActionResult StudentSelfEdit(string cid, string mid, string sid)
        {
            EvalutionViewModel SelfEVM = new EvalutionViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sname = db.Students.Find(sid).SName;
            var evalution = db.EvalutionResponse.Where(r => r.SID == SID && r.EvaluatorSID == SID && r.MID == mid);
            var qids = evalution.Select(r => r.DQID).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "自評互評").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            
            SelfEVM.DefaultQuestion = db.DefaultQuestions.Where(q => q.Class == "自評互評").Include(q => q.DefaultOptions);
            SelfEVM.MID = mid;
            SelfEVM.CID = cid;
            SelfEVM.SID = SID;
            SelfEVM.EvaluatorSID = SID;
            SelfEVM.CName = cname;
            SelfEVM.MName = mname;
            SelfEVM.SName = sname;
            SelfEVM.IsDiscuss = misChat;
            SelfEVM.SelfPeerAswer = evalution.ToList();

            return View(SelfEVM);
        }

        [HttpPost]
        public ActionResult StudentSelfEdit([System.Web.Http.FromBody] EvalutionViewModel groupVM, string cid, string mid, string sid)
        {

            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var MID = mid;
            var CID = cid;
            foreach (var qr in groupVM.ERs)
            {
                var response = db.EvalutionResponse.Where(r => r.SID == SID && r.EvaluatorSID == SID && r.MID == mid && r.DQID == qr.qid).ToList()[0];
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = SID;
                response.CID = cid;
                response.MID = mid;
                response.EvaluatorSID = SID;

                db.Entry(response).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                groupVM.CID = cid;
                groupVM.Groups = db.Groups.Where(g => g.CID == cid).ToList();

                return Json("suc");
            }

            return View(groupVM);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentPeerEvalution(string sid, string mid, string cid, string gid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sname = db.Students.Find(sid).SName;
            var evalution = db.EvalutionResponse.Where(r => r.SID == sid && r.EvaluatorSID == SID && r.MID == mid);
            var qids = evalution.Select(r => r.DQID).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "自評互評").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            if (questions.Any())
            {
                return RedirectToAction("StudentPeerER", "Student", new { sid, cid, mid, evasid = SID });
            }
            else
            {
                EvalutionViewModel PeerEVM = new EvalutionViewModel();
                PeerEVM.DefaultQuestion = db.DefaultQuestions.Where(q => q.Class == "自評互評").Include(q => q.DefaultOptions);
                PeerEVM.MID = mid;
                PeerEVM.CID = cid;
                PeerEVM.SID = sid;
                PeerEVM.EvaluatorSID = SID;
                PeerEVM.CName = cname;
                PeerEVM.MName = mname;
                PeerEVM.SName = sname;
                PeerEVM.IsDiscuss = misChat;

                return View(PeerEVM);
            }
        }

        [HttpPost]
        public ActionResult StudentPeerEvalution([System.Web.Http.FromBody] EvalutionViewModel evalution, string mid, string cid)  //填表單送出的Post
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var evaSID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sid = evalution.SID;
            foreach (var qr in evalution.ERs)
            {
                var response = new EvalutionResponse();
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = sid;
                response.EvaluatorSID = evaSID;
                response.MID = mid;
                response.CID = cid;
                db.EvalutionResponse.Add(response);
            }
            db.SaveChanges();

            return Json(new { redirectToUrl = Url.Action("Index", "PeerAssessments", new { cid = evalution.CID, mid = evalution.MID }) });

        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentPeerER(EvalutionViewModel evalution, string sid, string cid, string mid, string evasid)  ///學生已填過目標設置
        {
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var sname = db.Students.Find(sid).SName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            evalution.IsDiscuss = misChat;
            evalution.DefaultQuestion = db.DefaultQuestions.Where(q => q.Class == "自評互評").Include(q => q.EvalutionResponses);
            evalution.MID = mid;
            evalution.CID = cid;
            evalution.SID = sid;
            evalution.EvaluatorSID = evasid;
            evalution.CName = cname;
            evalution.MName = mname;
            evalution.SName = sname;

            return View(evalution);
        }

        public ActionResult StudentPeerEdit(string cid, string mid, string sid, string evasid)
        {
            EvalutionViewModel PeerEVM = new EvalutionViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sname = db.Students.Find(sid).SName;
            var evalution = db.EvalutionResponse.Where(r => r.SID == sid && r.EvaluatorSID == SID && r.MID == mid);
            var qids = evalution.Select(r => r.DQID).ToList();
            var questions = db.DefaultQuestions.Where(q => qids.Contains(q.DQID) && q.Class == "自評互評").ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            
            PeerEVM.DefaultQuestion = db.DefaultQuestions.Where(q => q.Class == "自評互評").Include(q => q.DefaultOptions);
            PeerEVM.MID = mid;
            PeerEVM.CID = cid;
            PeerEVM.SID = sid;
            PeerEVM.EvaluatorSID = SID;
            PeerEVM.CName = cname;
            PeerEVM.MName = mname;
            PeerEVM.SName = sname;
            PeerEVM.IsDiscuss = misChat;
            PeerEVM.SelfPeerAswer = evalution.ToList();

            return View(PeerEVM);
        }

        [HttpPost]
        public ActionResult StudentPeerEdit([System.Web.Http.FromBody] EvalutionViewModel groupVM, string cid, string mid, string evasid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var evaSID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var sid = groupVM.SID;
            var MID = mid;
            var CID = cid;
            foreach (var qr in groupVM.ERs)
            {
                var response = db.EvalutionResponse.Where(r => r.SID == sid && r.EvaluatorSID == evaSID && r.MID == mid && r.DQID == qr.qid).ToList()[0];
                response.DQID = qr.qid;
                response.Answer = qr.response;
                response.SID = sid;
                response.EvaluatorSID = evaSID;
                response.MID = mid;
                response.CID = cid;

                db.Entry(response).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                groupVM.CID = cid;
                groupVM.Groups = db.Groups.Where(g => g.CID == cid).ToList();

                return Json("suc");
            }

            return View(groupVM);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult StudentGroupEvalution(string mid, string cid, int gid)
        {
            EvalutionViewModel GroupEVM = new EvalutionViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var evalution = db.GroupERs.Where(r => r.GID == gid && r.EvaluatorSID == SID && r.MID == mid);
            var qids = evalution.Select(r => r.GQID).ToList();
            var questions = db.GroupOptions.Where(q => qids.Contains(q.GQID)).ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var gname = db.Groups.Find(gid).GName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
            var code = db.StudentCodes.Find(cid, mid, gid);
            var readcode = new TextIO();
            if (code != null)
            {
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                string basePath = $"{filePath}{code.CodePath}";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (!Path.HasExtension(basePath)) //沒有包含副檔名
                {
                    GroupEVM.IsCodeImg = false;
                    string readcodepath = $"{basePath}.txt";
                    GroupEVM.CodeText = readcode.readCodeText(readcodepath);
                }
                else  //有副檔名
                {
                    GroupEVM.CodePath = code.CodePath;
                    GroupEVM.IsCodeImg = true;
                }
            }
            if (pt != null)
            {
                GroupEVM.DrawingImgPath = pt;
            }
            if (questions.Any())
            {
                return RedirectToAction("StudentGroupER", "Student", new { cid, mid, gid });
            }        
            else
            { 
                GroupEVM.GroupQuestion = db.GroupQuestions.Include(q => q.GroupOptions);
                GroupEVM.MID = mid;
                GroupEVM.GID = gid;
                GroupEVM.CID = cid;
                GroupEVM.CName = cname;
                GroupEVM.MName = mname;
                GroupEVM.GName = gname;
                GroupEVM.IsDiscuss = misChat;

                return View(GroupEVM);
            }
        }

        [HttpPost]
        public ActionResult StudentGroupEvalution([System.Web.Http.FromBody] EvalutionViewModel evalution, string mid,string cid,int gid)  //填表單送出的Post
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            foreach (var qr in evalution.GRs)
            {
                var response = new GroupER();
                response.GQID = qr.qid;
                response.Answer = qr.response;
                response.GID = gid;
                response.EvaluatorSID = SID;
                response.CID = cid;
                response.MID = mid;

                db.GroupERs.Add(response);
            }
            db.SaveChanges();

            return Json(new { redirectToUrl = Url.Action("GroupEvalution", "PeerAssessments", new { gid = evalution.GID, cid = evalution.CID, mid = evalution.MID }) });
        }

        public ActionResult StudentGroupER(string cid, string mid, int gid)  ///學生已填過組間評價
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var gname = db.Groups.Find(gid).GName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
            var code = db.StudentCodes.Find(cid, mid, gid);
            var readcode = new TextIO();
            EvalutionViewModel evalution = new EvalutionViewModel();
            if (code != null)
            {
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                string basePath = $"{filePath}{code.CodePath}";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (!Path.HasExtension(basePath)) //沒有包含副檔名
                {
                    evalution.IsCodeImg = false;
                    string readcodepath = $"{basePath}.txt";
                    evalution.CodeText = readcode.readCodeText(readcodepath);
                }
                else  //有副檔名
                {
                    evalution.CodePath = code.CodePath;
                    evalution.IsCodeImg = true;
                }
            }
            if (pt != null)
            {
                evalution.DrawingImgPath = pt;
            }
            evalution.IsDiscuss = misChat;
            evalution.GroupQuestion = db.GroupQuestions.Include(q => q.GroupERs);
            evalution.SID = SID;
            evalution.MID = mid;
            evalution.CID = cid;
            evalution.GID = gid;
            evalution.CName = cname;
            evalution.MName = mname;
            evalution.GName = gname;

            return View(evalution);
        }

        public ActionResult StudentGroupEdit(string cid, int gid, string mid)
        {
            EvalutionViewModel GroupEVM = new EvalutionViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var evalution = db.GroupERs.Where(r => r.GID == gid && r.EvaluatorSID == SID && r.MID == mid);
            var qids = evalution.Select(r => r.GQID).ToList();
            var questions = db.GroupOptions.Where(q => qids.Contains(q.GQID)).ToList();
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var gname = db.Groups.Find(gid).GName;
            var misChat = db.Missions.Find(mid).IsDiscuss;
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
            var code = db.StudentCodes.Find(cid, mid, gid);
            var readcode = new TextIO();
            GroupEVM.GroupQuestion = db.GroupQuestions.Include(q => q.GroupOptions);
            GroupEVM.MID = mid;
            GroupEVM.GID = gid;
            GroupEVM.CID = cid;
            GroupEVM.CName = cname;
            GroupEVM.MName = mname;
            GroupEVM.GName = gname;
            GroupEVM.IsDiscuss = misChat;
            GroupEVM.Aswer = evalution.ToList();
            if (code != null)
            {
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string readcodepath = $"{filePath}{code.CodePath}.txt";
                GroupEVM.CodeText = readcode.readCodeText(readcodepath);
            }
            if (pt != null)
            {
                GroupEVM.DrawingImgPath = pt;
            }
           
            return View(GroupEVM);
        }

        [HttpPost]
        public ActionResult StudentGroupEdit([System.Web.Http.FromBody] EvalutionViewModel groupVM, string cid, int gid, string mid)
        {

            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var SID = claims.Claims.Where(x => x.Type == "SID").SingleOrDefault().Value;
            var MID = mid;
            var CID = cid;
            foreach (var qr in groupVM.GRs)
            {
                var response = db.GroupERs.Where(r => r.GID == gid && r.EvaluatorSID == SID && r.MID == mid && r.GQID == qr.qid).ToList()[0];
                response.GQID = qr.qid;
                response.Answer = qr.response;
                response.GID = gid;
                response.EvaluatorSID = SID;
                response.CID = cid;
                response.MID = mid;
                db.GroupERs.Add(response);
                db.Entry(response).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                groupVM.CID = cid;
                groupVM.Groups = db.Groups.Where(g => g.CID == cid).ToList();

                return Json("suc");
            }

            return View(groupVM);
        }
    }
}
