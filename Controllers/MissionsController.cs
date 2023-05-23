using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using LMSweb.ViewModel;

namespace LMSweb.Models
{
    [Authorize]
    public class MissionsController : Controller
    {
        private LMSmodel db = new LMSmodel();
        public ActionResult Index(string cid)
        {
            if (cid is null)
            {
                var data = (ClaimsIdentity)User.Identity;
                var role = data.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault();
                return RedirectToAction("Home", role.Value);
            }
            var mission_datas = db.Missions
                .Where(c => c.CID == cid)
                .Select(m => new MissionData()
                {
                    MID = m.MID,
                    Name = m.MName,
                    StartDate = m.Start,
                    EndDate = m.End
                }).ToList();
            var course_data = db.Courses.Where(c => c.CID == cid).FirstOrDefault();

            var mission_list = new MissionIndexViewModel()
            {
                CourseID = course_data.CID,
                CourseName = course_data.CName,
                TestType = course_data.TestType,
                Missions = mission_datas
            };

            return View(mission_list);
        }

        public ActionResult Details(string mid,string cid)
        {
            var data = (from m in db.Missions
                       from c in db.Courses
                       where m.CID == c.CID && m.MID == mid && c.CID == cid
                       select new MissionDetailViewModel
                       {
                           MissionID = m.MID,
                           CourseID = m.CID,
                           CourseName = c.CName,
                           TestType = c.TestType,
                           Name = m.MName,
                           Content = m.MDetail,
                           StartDate = m.Start,
                           EndDate = m.End,
                           CurrentAction = m.CurrentAction,
                           IsAssess = m.IsAssess,
                           IsCoding = m.IsCoding,
                           IsDiscuss = m.IsDiscuss,
                           IsDrawing = m.IsDrawing,
                           IsGoalSetting = m.IsGoalSetting,
                           IsGReflect = m.IsGReflect,
                           IsReflect = m.IsReflect,
                           Is_Journey = m.Is_Journey,
                           IsAddMetacognition = c.IsAddMetacognition,
                           IsAddPeerAssessmemt = c.IsAddPeerAssessmemt
                       }).FirstOrDefault();
            if(data != null && data.CurrentAction is null)
            {
                data.CurrentAction = DefaultCurrentStatus(data.TestType);
            }
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(string cid)
        { 
            var course = db.Courses.Find(cid);

            var createModel = new MissionCreateViewModel
            {
                CourseID = course.CID,
                CourseName = course.CName
            };
            return View(createModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(string cid,MissionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var test_type = db.Courses.Where(x => x.CID == cid).Select(x => x.TestType).FirstOrDefault();
                var missionData = new Mission
                {
                    CID = cid,
                    MID = $"M{DateTime.Now.ToString("yyMMddHHmmss")}",
                    MName = model.Name,
                    MDetail = model.Contents,
                    Start = model.StartDate.Replace("T", " "),
                    End = model.EndDate.Replace("T", " "),
                    CurrentAction = DefaultCurrentStatus(test_type)
                };
                model.CourseID = cid;
                db.Missions.Add(missionData);
                db.SaveChanges();
                return RedirectToAction("Index", new { cid = model.CourseID });
            }
            return View(model);
        }

        [NonAction]
        private string DefaultCurrentStatus(int type)
        {
            var table = new Dictionary<int, string>();
            table[0] = "00";
            table[1] = "0000";
            table[2] = "000";
            table[3] = "0000";
            table[4] = "0000";
            table[5] = "000000";
            return table[type];
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(string mid, string cid)
        {
            var missionData = (from mission in db.Missions
                              from course in db.Courses
                              where mission.CID == course.CID && mission.MID == mid && mission.CID == cid
                              select new MissionCreateViewModel
                              {
                                  CourseID = course.CID,
                                  CourseName = course.CName,
                                  MID = mission.MID,
                                  Name = mission.MName,
                                  Contents = mission.MDetail,
                                  StartDate = mission.Start,
                                  EndDate = mission.End                                  
                              })
                              .FirstOrDefault();
            return View(missionData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(string mid, string cid, MissionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var original = db.Missions
                    .Where(m=>m.CID == cid && m.MID == mid)
                    .FirstOrDefault();
                
                var test_type = db.Courses.Where(x => x.CID == cid).Select(x => x.TestType).FirstOrDefault();
                if (original.CurrentAction is null)
                {
                    original.CurrentAction = DefaultCurrentStatus(test_type);
                }
                var newMission = new Mission
                {
                    CID = cid,
                    MID = mid,
                    MName = model.Name,
                    MDetail = model.Contents,
                    Start = model.StartDate.Replace("T", " "),
                    End = model.EndDate.Replace("T", " "),
                    CurrentAction = original.CurrentAction
                };
                db.Entry(original).CurrentValues.SetValues(newMission);
                db.SaveChanges();
                return RedirectToAction("Index", new { cid = newMission.CID });
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(string mid, string cid)
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
            var cname = db.Courses.Find(cid).CName;
            model.CID = cid;
            model.CName = cname;
            model.mis = mission;

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public ActionResult DeleteConfirmed(string mid)
        {

            Mission mission = db.Missions.Find(mid);
            var cid = mission.CID;
            var learningBehavior = db.LearnB.Where(l => l.mission.MID == mid);
            var teacherA = db.TeacherA.Where(t => t.MID == mid);
            var question = db.Questions.Where(q => q.MID == mid);
            var peerA = db.PeerA.Where(p => p.MID == mid);
            var option = db.Options.Where(o => o.Question.MID == mid);
            var response = db.Responses.Where(r => r.MID == mid);
            var stuDraw = db.StudentDraws.Where(sd => sd.MID == mid);
            var stuCode = db.StudentCodes.Where(sc => sc.MID == mid);
            var ger = db.GroupERs.Where(gr => gr.MID == mid);
            var eresponse = db.EvalutionResponse.Where(er => er.MID == mid);

            db.Questions.RemoveRange(question);
            db.LearnB.RemoveRange(learningBehavior);
            db.TeacherA.RemoveRange(teacherA);
            db.PeerA.RemoveRange(peerA);
            db.Responses.RemoveRange(response);
            db.Options.RemoveRange(option);
            db.StudentDraws.RemoveRange(stuDraw);
            db.StudentCodes.RemoveRange(stuCode);
            db.GroupERs.RemoveRange(ger);
            db.EvalutionResponse.RemoveRange(eresponse);
            db.Missions.Remove(mission);
            db.SaveChanges();
            return RedirectToAction("Index", new { cid });
        }
        
        [Authorize(Roles = "Teacher")]
        public ActionResult SelectCourses(string cid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "UID").FirstOrDefault();   //抓出當初記載Claims陣列中的TID
            var tid = claimData.Value; //取值(因為只有一筆)
            var courses = db.Courses.Where(c => c.TID == tid).Select(x => new TeacherHomeViewModel
            {
                CourseID = x.CID,
                CourseName = x.CName,
                TestType = x.TestType
            });
            ViewData["CurrentCourseID"] = cid;
            return View(courses.ToList());
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult SelectMissions(string selectedCID, string currentCID)
        {
            var tasklist = db.Missions.Where(m => m.CID == selectedCID).Select(s => new TaskData
            {
                TaskID = s.MID,
                Name = s.MName,
                TaskDetail = s.MDetail,
                StartDate = s.Start,
                EndDate = s.End
            }).ToList();

            var course = db.Courses.Find(selectedCID);
            var data = new SelectMissionViewModel
            {
                CurrentCourseID = currentCID,
                CourseID = course.CID,
                CourseName = course.CName,
                TestType = course.TestType,
                Missions = tasklist
            };
            return View(data);
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult Copy(string mid, string selectedCID,string currentCID)
        {
            var missionData = (from mission in db.Missions
                              from course in db.Courses
                              where mission.CID == course.CID && mission.MID == mid && course.CID == selectedCID
                              select new MissionCreateViewModel
                              {
                                  CourseID = course.CID,
                                  CourseName = course.CName,
                                  MID = mission.MID,
                                  Name = mission.MName,
                                  Contents = mission.MDetail,
                                  StartDate = mission.Start,
                                  EndDate = mission.End
                              }).FirstOrDefault();
            ViewData["CurrentCourseID"] = currentCID;
            return View(missionData);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Copy(string currentCID, MissionCreateViewModel formdata)
        {
            if (ModelState.IsValid)
            {
                var test_type = db.Courses.Where(x => x.CID == currentCID).Select(x => x.TestType).FirstOrDefault();
                var missionData = new Mission
                {
                    CID = currentCID,
                    MID = $"M{DateTime.Now.ToString("yyMMddHHmmss")}",
                    MName = formdata.Name,
                    MDetail = formdata.Contents,
                    Start = formdata.StartDate.Replace("T", " "),
                    End = formdata.EndDate.Replace("T", " "),
                    CurrentAction = DefaultCurrentStatus(test_type)
                };
                formdata.CourseID = currentCID;
                db.Missions.Add(missionData);
                db.SaveChanges();

                return RedirectToAction("Index", new {cid = formdata.CourseID});
            }

            return View(formdata);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult SwitchDrawing(string mid, string cid, string type, bool sw)
        {
            Mission mission = db.Missions.Find(mid);
            db.Entry(mission).State = EntityState.Modified;
            if (type == "is_Coding")
            {
                mission.IsCoding = sw;
            }
            else if(type == "is_Discuss")
            {
                mission.IsDiscuss = sw;
            }
            else if(type == "is_Drawing")
            {
                mission.IsDrawing = sw;
            }
            else if (type == "is_Assess")
            {
                mission.IsAssess = sw;
            }
            else if (type == "is_Goalsetting")
            {
                mission.IsGoalSetting = sw;
            }
            else if (type == "is_Reflect")
            {
                mission.IsReflect = sw;
            }
            else if (type == "is_GReflect")
            {
                mission.IsGReflect = sw;
            }
            else if (type == "is_Journey")
            {
                mission.Is_Journey = sw;
            }

            db.SaveChanges();

            return Json(new { Status = HttpStatusCode.OK , type = type, sw = sw});
        }
    }
}
