using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LMSweb.ViewModel;

namespace LMSweb.Models
{
    [Authorize(Roles = "Teacher")]
    public class MissionsController : Controller
    {
        private LMSmodel db = new LMSmodel();
        public ActionResult Index(string cid)
        {
            /* 盈琪寫的Code
             MissionViewModel model = new MissionViewModel();

            if (cid == null)
            {
                model.missions = db.Missions.Where(m => m.CID == cid);
                return View(model);
            }
            var course = db.Courses.Single(c => c.CID == cid);
            model.missions = db.Missions.Where(m => m.CID == cid);
            model.CID = course.CID;
            model.CName = course.CName;

            return View(model);*/

            if (cid == null)
            {
                return RedirectToAction("TeacherHomePage", "Teacher");
            }
            var mission_datas = db.Missions.Where(c => c.CID == cid).Select(m => new MissionData()
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
                Missions = mission_datas
            };

            return View(mission_list);
        }

        public ActionResult Details(string mid,string cid)
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
            var mname = db.Missions.Find(mid).MName;
            var course = db.Courses.Find(cid);
            var kps = mission.relatedKP.Split(',');
            model.KContents = new List<string>();
            for (int i = 0; i < kps.Length - 1; i++)
            {
                model.KContents.Add(db.KnowledgePoints.Find(int.Parse(kps[i])).KContent);
            }
            model.CID = cid;
            model.mis = mission;
            model.MID = mid;
            model.course = course;
            model.CName = course.CName;
            model.MName = mname;

            
           
            return View(model);
        }

        public JsonResult GetKnowledgeJSON(string cid, IEnumerable<int> SelectKnowledgeList = null)
        {
             return Json(new { Data = new MultiSelectList(db.KnowledgePoints.Where(kp => kp.CID == cid), "KID", "KContent", SelectKnowledgeList) }, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<SelectListItem> GetKnowledge(string cid, IEnumerable<int> SelectKnowledgeList=null)
        {
            return new MultiSelectList(db.KnowledgePoints.Where(kp => kp.CID == cid), "KID", "KContent", SelectKnowledgeList);
        }

        [HttpGet]
        public ActionResult Create(string cid)
        {
            var model = new MissionCreateViewModel();
            var cname = db.Courses.Find(cid).CName;
            model.KnowledgeList = GetKnowledge(cid);
            model.CID = cid;
            model.CName = cname;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MissionCreateViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var kps = db.KnowledgePoints.Where(x => model.SelectKnowledgeList.ToList().Contains(x.KID)).ToList();
                string kp_str = "";
                foreach (var kp in kps)
                {
                    kp_str += kp.KID.ToString() + ",";
                }
                model.mission.relatedKP = kp_str;
                model.mission.CID = model.CID;

                db.Missions.Add(model.mission);
                db.SaveChanges();
                
                return RedirectToAction("Index", new { cid = model.CID });
            }
            var vmodel = new MissionCreateViewModel();
            var cname = db.Courses.Find(model.CID).CName;
            vmodel.KnowledgeList = GetKnowledge(model.CID);
            vmodel.CID = model.CID;
            model.CName = cname;

            return View(vmodel);
        }

        [HttpGet]
        public ActionResult Edit(string mid, string cid)
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
            var vmodel = new MissionCreateViewModel();
            var cname = db.Courses.Find(cid).CName;
            vmodel.mission = mission;
            vmodel.KnowledgeList = GetKnowledge(cid, db.KnowledgePoints.Where(kp => kp.CID == mission.CID).Select(kp => kp.KID));
            vmodel.CID = cid;
            vmodel.CName = cname;

            return View(vmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MissionCreateViewModel model)
        {
            var mission = model.mission;
            if (ModelState.IsValid)
            {
                db.Entry(mission).State = EntityState.Modified;
                db.SaveChanges();

                var kps = db.KnowledgePoints.Where(x => model.SelectKnowledgeList.ToList().Contains(x.KID)).ToList();
                string kp_str = "";
                foreach (var kp in kps)
                {
                    kp_str += kp.KID.ToString() + ",";
                }
                model.mission.relatedKP = kp_str;
                mission.CID = model.CID;

                db.SaveChanges();

                return RedirectToAction("Index", new { cid = model.CID});
            }
            model.CID = mission.CID;
            return View(model);
        }
        [HttpGet]
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult SelectMissions(string cid, string missionCourse)
        {
            MissionViewModel model = new MissionViewModel();
            var cname = db.Courses.Find(cid).CName;
            model.missions = db.Missions.ToList();
            model.CID = cid;
            model.CName = cname;

            return View(model);
        }

        public ActionResult AddMissions(string mid, string cid)
        {
            Mission mission = db.Missions.Find(mid);  //old
            var model = new MissionCreateViewModel();   //new
            model.mission = new Mission();
            model.mission.MID = mission.MID;
            model.mission.Start = mission.Start;
            model.mission.End = mission.End;
            model.mission.MName = mission.MName;
            model.mission.MDetail = mission.MDetail;
            model.KnowledgeList = GetKnowledge(cid);
            model.mission.CID = cid;
            model.CID = cid;
            model.CName = mission.course.CName;
           
            return View(model);
        }

        [HttpPost]
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
