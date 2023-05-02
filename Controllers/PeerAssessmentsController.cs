using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using LMSweb.Models;
using LMSweb.ViewModel;

namespace LMSweb.Controllers
{
    public class PeerAssessmentsController : Controller
    {
        private LMSmodel db = new LMSmodel();

        [HttpGet]
        [Authorize(Roles = "Student")]
        public ActionResult Index(string mid, string cid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; 
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).Group;
            var sname = db.Students.Find(sid).SName;
            var mis = db.Missions.Find(mid);
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var pa = db.PeerA.SingleOrDefault(p => p.AssessedSID == sid && p.MID == mid);
            var gmodel = new GroupViewModel();
            var misChat = db.Missions.Find(mid).IsDiscuss;

            gmodel.Groups = db.Groups.Where(g => g.GID == stuG.GID).ToList();
            gmodel.PeerAssessment = pa;
            gmodel.CName = cname;
            gmodel.CID = cid;
            gmodel.MID = mid;
            gmodel.SID = sid;
            gmodel.MName = mname;
            gmodel.IsDiscuss = misChat;
            gmodel.SName = sname;
            gmodel.EvalutionResponse = db.EvalutionResponse.Where(sg => sg.EvaluatorSID == sid && sg.MID == mid).ToList();

            return View(gmodel);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeerAssessment peerAssessment = db.PeerA.Find(id);
            if (peerAssessment == null)
            {
                return HttpNotFound();
            }

            return View(peerAssessment);
        }

        public ActionResult Create(string sid, string mid, int gid, string cid)
        {
            GroupViewModel model = new GroupViewModel();
            model.MID = mid;
            model.GID = gid;
            model.SID = sid;
            model.CID = cid;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PEID,PeerA,CooperationScore,AssessedSID,MID")] PeerAssessment peerAssessment, string mid, int gid, string cid)
        {
            if (ModelState.IsValid)
            {
                db.PeerA.Add(peerAssessment);
                db.SaveChanges();

                var gmodel = new GroupViewModel();
                gmodel.MID = mid;
                gmodel.GID = gid;
                gmodel.CID = cid;

                return RedirectToAction("Index", gmodel);
            }

            return View(peerAssessment);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeerAssessment peerAssessment = db.PeerA.Find(id);
            if (peerAssessment == null)
            {
                return HttpNotFound();
            }

            return View(peerAssessment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PEID,PeerA,CooperationScore,AssessedSID")] PeerAssessment peerAssessment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(peerAssessment).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(peerAssessment);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeerAssessment peerAssessment = db.PeerA.Find(id);
            if (peerAssessment == null)
            {
                return HttpNotFound();
            }

            return View(peerAssessment);
        }

 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PeerAssessment peerAssessment = db.PeerA.Find(id);

            db.PeerA.Remove(peerAssessment);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Student")]
        public ActionResult GroupEvalution(int gid, string mid, string cid)
        {
            var gmodel = new GroupViewModel();
            gmodel.MID = mid;
            var mis = db.Missions.Find(mid);

            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();   //抓出當初記載Claims陣列中的SID
            var sid = claimData[0].Value;
            
           
            var stu = db.Students.Where(s => s.SID == sid);
            var stuC = db.Students.Find(sid).Course;
            var stuG = db.Students.Find(sid).Group;
            var misChat = db.Missions.Find(mid).IsDiscuss;

            gmodel.Courses = db.Courses.Where(c => c.CID == stuC.CID).ToList();
            gmodel.Groups = db.Groups.Where(g => g.GID != stuG.GID && g.CID == cid).ToList();

            var pa = db.PeerA.SingleOrDefault(p => p.AssessedSID == sid && p.MID == mid);
            gmodel.PeerAssessment = pa;
            var course = db.Courses.Single(c => c.CID == cid);
            var mname = db.Missions.Find(mid).MName;

            
            gmodel.CID = cid;
            gmodel.CName = course.CName;
            gmodel.IsDiscuss = misChat;
            gmodel.MName = mname;
            gmodel.GroupER = db.GroupERs.Where(sg => sg.MID == mid && sg.EvaluatorSID == sid).ToList();

            return View(gmodel);
        }

        public ActionResult PeerEvaluation()
        {
            return View();
        }
    }
}
