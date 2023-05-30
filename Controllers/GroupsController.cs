using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using LMSweb.Models;
using LMSweb.Service;
using LMSweb.ViewModel;

namespace LMSweb.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class GroupsController : Controller
    {
        private LMSmodel db = new LMSmodel();

        public ActionResult Index(string mid, string cid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var TID = claims.Claims.Where(x => x.Type == "UID").FirstOrDefault().Value;
            var gmodel = new GroupViewModel();
            gmodel.MID = mid;
            var mis = db.Missions.Find(mid);
            var mname = mis.MName;
            

            
            gmodel.Groups = db.Groups.Where(g => mis.CID == g.CID).ToList();
            gmodel.CID = cid;
            gmodel.CName = mis.Course.CName;
            gmodel.MName = mname;
            var stu = db.Students.Where(s => s.Group.CID == cid).ToList();
            gmodel.IsUploadDraw = db.StudentDraws.Where(sd => sd.MID == mid).ToList();
            gmodel.IsUploadCode = db.StudentCodes.Where(sc => sc.MID == mid).ToList();
            gmodel.GroupER = db.GroupERs.Where(sg => sg.MID == mid && sg.EvaluatorSID == TID).ToList();
            
            return View(gmodel);
        }
        public ActionResult CheckCoding(int gid, string cid, string mid)
        {
            DrawingViewModel dvmodel = new DrawingViewModel();
            

            dvmodel.CID = cid;
            dvmodel.GID = gid;
            dvmodel.MID = mid;
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
            dvmodel.DrawingImgPath = pt;

            return View(dvmodel);
        }
        public ActionResult CheckDrawing(int gid, string cid)
        {
            GroupViewModel model = new GroupViewModel();
            model.CID = cid;
            model.GID = gid;

            return View(model);
        }

        private string codefileSavedPath = WebConfigurationManager.AppSettings["CodePath"];

        [HttpGet]
        public ActionResult Assessment(int gid, string mid, string cid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var TID = claims.Claims.Where(x => x.Type == "UID").SingleOrDefault().Value;
            var evalution = db.GroupERs.Where(r => r.GID == gid && r.EvaluatorSID == TID && r.MID == mid);
            var qids = evalution.Select(r => r.GQID).ToList();
            var questions = db.GroupOptions.Where(q => qids.Contains(q.GQID)).ToList();

            if (questions.Any())
            {
                return RedirectToAction("StudentTeacherER", "Groups", new { cid, mid, gid });
            }
            else
            {
                EvalutionViewModel model = new EvalutionViewModel();
                model.MID = mid;
                model.GID = gid;
                model.CID = cid;

                var group = db.Groups.Single(g => g.GID == gid);
                var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
                var code = db.StudentCodes.Find(cid, mid, gid);
                var cname = db.Courses.Find(cid).CName;
                var gname = db.Groups.Find(gid).GName;
                var mname = db.Missions.Find(mid).MName;
                var readcode = new TextIO();

                model.GroupQuestion = db.GroupQuestions.Include(q => q.GroupOptions);
                model.MID = mid;
                model.GID = gid;
                model.CID = cid;
                model.TID = TID;
                model.CName = cname;
                model.MName = mname;
                model.GName = gname;
                if (code != null)
                {
                    string virtualBaseFilePath = Url.Content(codefileSavedPath);
                    string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string readcodepath = $"{filePath}{code.CodePath}.txt";
                    model.CodePath = code.CodePath;
                    model.CodeText = readcode.readCodeText(readcodepath);
                }
                if (pt != null)
                {
                    model.DrawingImgPath = pt;
                }
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Assessment([System.Web.Http.FromBody] EvalutionViewModel groupVM, int gid, string mid, string cid)
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var TID = claims.Claims.Where(x => x.Type == "TID").SingleOrDefault().Value;
            int GID = gid;
            var MID = mid;
            var CID = cid;

            foreach (var qr in groupVM.TRs)
            {
                var response = new GroupER();

                response.GQID = qr.qid;
                response.Answer = qr.response;
                response.Comments = qr.comments;
                response.GID = GID;
                response.EvaluatorSID = TID;
                response.CID = CID;
                response.MID = MID;
                db.GroupERs.Add(response);
            }

            db.SaveChanges();

            if (ModelState.IsValid)
            {
                var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
                var code = db.StudentCodes.Find(cid, mid, gid);
                var readcode = new TextIO();
                
                if (code != null)
                {
                    string virtualBaseFilePath = Url.Content(codefileSavedPath);
                    string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string readcodepath = $"{filePath}{code.CodePath}.txt";
                    groupVM.CodeText = readcode.readCodeText(readcodepath);
                    groupVM.IsUploadCode = db.StudentCodes.Where(sc => sc.MID == mid).ToList();

                }
                else
                {
                    groupVM.IsUploadCode = null;
                }
                if (pt != null)
                {
                    groupVM.DrawingImgPath = pt;
                    groupVM.IsUploadDraw = db.StudentDraws.Where(sd => sd.MID == mid).ToList();
                }
                else
                {
                    groupVM.IsUploadDraw = null;
                }
                var course = db.Courses.Find(cid).CName;

                groupVM.CID = cid;
                groupVM.MID = mid;
                groupVM.CName = course;
                groupVM.Groups = db.Groups.Where(g => g.CID == cid).ToList();


                return Json(new { redirectToUrl = Url.Action("Index", "Groups", new { gid = groupVM.GID, cid = groupVM.CID, mid = groupVM.MID }) });
            }

            return View(groupVM);
        }
        
        public ActionResult Edit(int? id, string cid, int gid, string mid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeacherAssessment teacherAssessment = db.TeacherA.Find(id);
            var groupVM = new GroupViewModel();
            groupVM.CID = cid;
            groupVM.MID = mid;
            groupVM.GID = gid;

            var code = db.StudentCodes.Find(cid, mid, gid);
            var course = db.Courses.Find(cid);
            var gname = db.Groups.Find(gid);
            groupVM.CName = course.CName;
            groupVM.GName = gname.GName;
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
            var readcode = new TextIO();
            
            
            if (teacherAssessment != null)
            {
                groupVM.TeacherAssessment = teacherAssessment;
            }
            if (code != null )
            {
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string readcodepath = $"{filePath}/{code.CodePath}.txt";
                groupVM.CodeText = readcode.readCodeText(readcodepath);
                groupVM.IsUploadCode = db.StudentCodes.Where(sc => sc.MID == mid).ToList();

            }
            if (pt != null)
            {
                groupVM.DrawingImgPath = pt;
                groupVM.IsUploadDraw = db.StudentDraws.Where(sd => sd.MID == mid).ToList();
            }

            return View(groupVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GroupViewModel groupVM, string cid, int gid, string mid)
        {
            var teacherAssessment = groupVM.TeacherAssessment;

            teacherAssessment.MID = mid;
            teacherAssessment.GID = gid;
            teacherAssessment.CID = cid;
            if (ModelState.IsValid)
            {
                db.Entry(teacherAssessment).State = EntityState.Modified;
                db.SaveChanges();

                groupVM.CID = cid;
                groupVM.Groups = db.Groups.Where(g => g.CID == cid).ToList();

                return RedirectToAction("Index", groupVM);
            }

            return View(groupVM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult StudentTeacherER(string cid, string mid, int gid)  ///學生已填過組間評價
        {
            EvalutionViewModel evalution = new EvalutionViewModel();
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var TID = claims.Claims.Where(x => x.Type == "TID").SingleOrDefault().Value;
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
            evalution.MID = mid;
            evalution.CID = cid;
            evalution.GID = gid;
            evalution.CName = cname;
            evalution.MName = mname;
            evalution.GName = gname;
            evalution.TID = TID;

            return View(evalution);
        }
        public ActionResult TeacherEdit( string cid, int gid, string mid)
        {
            
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var TID = claims.Claims.Where(x => x.Type == "TID").SingleOrDefault().Value;
            var evalution = db.GroupERs.Where(r => r.GID == gid && r.EvaluatorSID == TID && r.MID == mid);
            var qids = evalution.Select(r => r.GQID).ToList();
            var rids = evalution.Select(r => r.RID).ToList();
            var questions = db.GroupOptions.Where(q => qids.Contains(q.GQID)).ToList();
            EvalutionViewModel model = new EvalutionViewModel();
            model.MID = mid;
            model.GID = gid;
            model.CID = cid;

            var group = db.Groups.Single(g => g.GID == gid);
            var pt = db.StudentDraws.Where(p => p.GID == gid && p.MID == mid).Select(p => p.DrawingImgPath).SingleOrDefault();
            var code = db.StudentCodes.Find(cid, mid, gid);
            var cname = db.Courses.Find(cid).CName;
            var gname = db.Groups.Find(gid).GName;
            var mname = db.Missions.Find(mid).MName;
            var readcode = new TextIO();

            
            model.GroupQuestion = db.GroupQuestions.Include(q => q.GroupOptions);
            model.MID = mid;
            model.GID = gid;
            model.CID = cid;
            model.TID = TID;
            model.CName = cname;
            model.MName = mname;
            model.GName = gname;
            model.Aswer = evalution.ToList();

            if (code != null)
            {
                string virtualBaseFilePath = Url.Content(codefileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string readcodepath = $"{filePath}{code.CodePath}.txt";
                model.CodePath = code.CodePath;
                model.CodeText = readcode.readCodeText(readcodepath);
            }
            if (pt != null)
            {
                model.DrawingImgPath = pt;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult TeacherEdit([System.Web.Http.FromBody] EvalutionViewModel groupVM, string cid, int gid, string mid)
        {
            
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
            var TID = claims.Claims.Where(x => x.Type == "TID").SingleOrDefault().Value;
            int GID = gid;
            var MID = mid;
            var CID = cid;

            foreach (var qr in groupVM.TRs)
            {
                var teacherAssessment = db.GroupERs.Where(r => r.GID == gid && r.EvaluatorSID == TID && r.MID == mid && r.GQID == qr.qid).ToList()[0];
                teacherAssessment.GQID = qr.qid;
                teacherAssessment.Answer = qr.response;
                teacherAssessment.Comments = qr.comments;
                teacherAssessment.GID = GID;
                teacherAssessment.EvaluatorSID = TID;
                teacherAssessment.CID = CID;
                teacherAssessment.MID = MID;

                db.Entry(teacherAssessment).State = EntityState.Modified;
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
