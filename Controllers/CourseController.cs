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
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using LMSweb.Infrastructure.Helpers;


namespace LMSweb.Controllers
{
    [RoutePrefix("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class CourseController : Controller
    {
        private LMSmodel db = new LMSmodel();

        // GET: StudentCreate
        public ActionResult StudentCreate(string cid)
        {
            var vmodel = new StudentViewModel();
            vmodel.CID = cid;
            var course = db.Courses.Where(c => c.CID == cid).Single();

            vmodel.CName = course.CName;
            
            return View(vmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentCreate(StudentViewModel vmodel)
        {
            if (ModelState.IsValid)
            {
                var _student = new Student()
                {
                    CID = vmodel.student.CID,
                    SID = vmodel.student.SID,
                    SName = vmodel.student.SName,
                    Sex = vmodel.student.Sex,
                    SPassword = vmodel.student.SID,
                    IsLeader = false,

                    GID = 6
                };

                var _User = new User()
                {
                    ID = vmodel.student.SID,
                    UPassword = vmodel.student.SID,
                    Name = vmodel.student.SName,
                    Gender = vmodel.student.Sex,
                    RoleName = "Student"
                };

                db.Students.Add(vmodel.student);
                db.SaveChanges();   
                db.Users.Add(_User);
                db.SaveChanges();
                return RedirectToAction("StudentManagement",new { cid = vmodel.student.CID } );
            }
            vmodel.CID = vmodel.student.CID;
            var course = db.Courses.Where(c => c.CID == vmodel.student.CID).Single();
            
            vmodel.CName = course.CName;                                     

            return View(vmodel);
        }
        // GET: StudentEdit
        public ActionResult StudentEdit(string sid, string cid)
        {
            if (sid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(sid);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentEdit([Bind(Include = "SID,CourseID,SName,SPassword,Sex,Score")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentManagement", "Course", new { student.CID});
            }

            return View(student);
        }

        // GET: StudentDelete
        public ActionResult StudentDelete(string sid)
        {
            if (sid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(sid);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("StudentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult StudentDeleteConfirmed(string sid, string cid)
        {
            Student student = db.Students.Find(sid);
            db.Students.Remove(student);
            db.SaveChanges();

            return RedirectToAction("StudentManagement", "Course", new { cid });
        }

        /*上傳EXCEL檔*/
        private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string cid)
        {
            JObject jo = new JObject();
            string result = string.Empty;

            if (file == null)
            {
                jo.Add("Result", false);
                jo.Add("Msg", "請上傳檔案!");
                jo.Add("error", "請上傳檔案!");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }
            if (file.ContentLength <= 0)
            {
                jo.Add("Result", false);
                jo.Add("Msg", "請上傳正確的檔案.");
                jo.Add("error", "請上傳正確的檔案.");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }

            string fileExtName = Path.GetExtension(file.FileName).ToLower();

            if (!fileExtName.Equals(".xls", StringComparison.OrdinalIgnoreCase) && !fileExtName.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                jo.Add("Result", false);
                jo.Add("Msg", "請上傳 .xls 或 .xlsx 格式的檔案");
                jo.Add("error", "請上傳 .xls 或 .xlsx 格式的檔案");
                result = JsonConvert.SerializeObject(jo);
                return Content(result, "application/json");
            }
            try
            {
                var uploadResult = this.FileUploadHandler(file);
                result = this.Import(uploadResult, cid);
            }
            catch (Exception ex)
            {
                jo.Add("Result", false);
                jo.Add("Msg", ex.Message);
                jo.Add("error", ex.Message);
                result = JsonConvert.SerializeObject(jo);
            }

            return Content(result, "application/json");
        }

        private string FileUploadHandler(HttpPostedFileBase file)
        {
            string result;

            if (file == null)
            {
                throw new ArgumentNullException("file", "上傳失敗：沒有檔案！");
            }
            if (file.ContentLength <= 0)
            {
                throw new InvalidOperationException("上傳失敗：檔案沒有內容！");
            }
            try
            {
                string virtualBaseFilePath = Url.Content(fileSavedPath);
                string filePath = HttpContext.Server.MapPath(virtualBaseFilePath);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string newFileName = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(file.FileName).ToLower());
                string fullFilePath = Path.Combine(Server.MapPath(fileSavedPath), newFileName);
                file.SaveAs(fullFilePath);
                result = newFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private string Import(string savedFileName, string cid)
        {
            var jo = new JObject();
            string result;
            try
            {
                var fileName = string.Concat(Server.MapPath(fileSavedPath), "/", savedFileName);
                var importStudents = new List<Student>();
                var helper = new ImportDataHelper();
                var checkResult = helper.CheckImportData(fileName, importStudents);
                jo.Add("Result", checkResult.Success);
                jo.Add("Msg", checkResult.Success ? string.Empty : checkResult.ErrorMessage);

                if (checkResult.Success)
                {
                    helper.SaveImportData(importStudents, cid);//儲存匯入的資料
                }
                result = JsonConvert.SerializeObject(jo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        protected override void Dispose(bool disposing) 
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private List<SelectListItem> GetStudent(string cid, IEnumerable<int> SelectStudentList = null)
        {
            // 註解是原本的寫法
            //return new MultiSelectList(db.Students.Where(x => x.Group == null && x.CID == cid), "SID", "SName", SelectStudentList).ToList();
            return new MultiSelectList(db.Students.Where(x => (x.Group == null || x.GID == 6) && x.CID == cid), "SID", "SName", SelectStudentList).ToList();
        }

        // GET: StudentGroup
        public ActionResult StudentGroup(string cid)
        {            
            var vmodel = new GroupCreateViewModel();
            vmodel.StudentList = GetStudent(cid);
            vmodel.students = db.Students.Where(x => x.Group != null && x.CID == cid).ToList();
            vmodel.CID = cid;
            
            var course = db.Courses.Where(c => c.CID == cid).Single();
            vmodel.CName = course.CName;

            vmodel.groups = db.Groups.Where(g =>g.CID == cid).ToList();

            return View(vmodel);
        }
        
        [HttpPost]
        public ActionResult StudentGroup(string GName, List<string> StudentList, string cid)
        {
            if (ModelState.IsValid)
            {
                Group group = new Group();
                group.GName = GName;
                group.CID = cid;
                group.Students = db.Students.Where(x => StudentList.Contains(x.SID)).ToList();
                db.Groups.Add(group);

                db.SaveChanges();

                return new HttpStatusCodeResult(200);
            }
            var vmodel = new GroupCreateViewModel();
            vmodel.StudentList = GetStudent(vmodel.CID);

            return View(vmodel);
        }
        
        [HttpPost]
        public ActionResult EditGname(int gid, string gName)
        {
            
            if (ModelState.IsValid)
            {
                Group group = db.Groups.Find(gid);
                group.GName = gName;
                db.Entry(group).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("StudentGroup", "Course", new { group.CID });
            }

            return View();
        }

        public ActionResult StudentManagement(string cid)
        {
            if (cid == null)
            {
                return new HttpStatusCodeResult(404);
            }

            CourseViewModel model = new CourseViewModel();
            var course = db.Courses.Single(c => c.CID == cid);

            model.CID = course.CID;
            model.CName = course.CName;
            model.students = course.Students;
            ViewBag.CID = cid;

            return View(model);
        }

        [HttpPost]
        public ActionResult GroupN(int n, string cid)
        {
            
            var stus = GetRandomElements(db.Students.Where(x => (x.Group == null || x.GID == 6) && cid == x.CID).ToList());
            List<Group> groups = new List<Group>();
            var left_s = stus.Count % n;
            

            for (int i = 1; i <= n; i++)
            {
                var g = new Group();
                g.GName = "第" + i.ToString() + "組";
                g.Students = new List<Student>();
                g.CID = cid;
                groups.Add(g);
            }
            int g_idx = 0;
            
            for (int i = 0; i < stus.Count; i++)
            {
                groups[g_idx].Students.Add(stus[i]);
                g_idx++;
                g_idx = g_idx % n;
            }

            for (int i = 0; i < n; i++)
            {
                db.Groups.Add(groups[i]);
            }

            db.SaveChanges();

            //for (int i = 0; i < stus.Count; i++)
            //{
            //    var GIDList = db.Groups.Where(g => g.CID == cid).Select(g => g.GID).ToList();
            //    var stu = db.Students.Where(s => s.SID == stus[i].SID).Single();
            //    stu.GID = GIDList[i % n];
            //    db.SaveChanges();
            //}

            return RedirectToAction("StudentGroup", new { cid });
        }

        public static List<t> GetRandomElements<t>(IEnumerable<t> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public ActionResult GroupDelete(int groupId)
        {
            Group group = db.Groups.Find(groupId);
 
            if (group == null)
            {
                return HttpNotFound();
            }

            var GroupStu = db.Students.Where(x => x.GID == groupId);
            foreach (var stu in GroupStu)
            {
                stu.GID = 6;
            }

            var cid = group.CID;
            group.Students.Clear();
            db.Groups.Remove(group);
            var learnb = db.LearnB.Where(x => x.group.GID == groupId);
            db.LearnB.RemoveRange(learnb);
            var studentCode = db.StudentCodes.Where(sc => sc.group.GID == groupId);
            db.StudentCodes.RemoveRange(studentCode);
            var studentDraw = db.StudentDraws.Where(sd => sd.Group.GID == groupId);
            db.StudentDraws.RemoveRange(studentDraw);

            db.SaveChanges();

            return RedirectToAction("StudentGroup", new { cid=cid} );
        }

        public ActionResult GroupStudentDelete(string groupStuId)
        {
            Student student = db.Students.Find(groupStuId);

            if (student == null)
            {
                return HttpNotFound();
            }

            Group group = student.Group;
            group.Students.Remove(student);
            student.GID = 6;

            db.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        public ActionResult AddStuToOtherGroup(int gid, List<string> StudentList, string CID)
        {
           

            if (ModelState.IsValid)
            {
                foreach(var sid in StudentList)
                {
                    Student student = db.Students.Find(sid);
                    Group group = db.Groups.Find(gid);
                    group.Students.Add(student);
 
                }
                
                db.SaveChanges();

                return new HttpStatusCodeResult(200);
            }
            var vmodel = new GroupCreateViewModel();
            vmodel.StudentList = GetStudent(vmodel.CID);

            return View(vmodel);                                                                                                                                                                                                         
        }

        public ActionResult StudentSurvey (string cid, string mid)
        {
            var svmodel = new StudentSurveyViewModel();
            var gsquestion = db.DefaultQuestions.Where(q => q.Class == "目標設置").Select(q => q.DQID).ToList();
            var gsrespon = db.Responses.Where(r => gsquestion.Contains(r.DQID)).ToList();
            var requestion = db.DefaultQuestions.Where(q => q.Class == "自我反思").Select(q => q.DQID).ToList();
            var rerespon = db.Responses.Where(r => requestion.Contains(r.DQID)).ToList();
            var equestion = db.DefaultQuestions.Where(q => q.Class == "自評互評").Select(q => q.DQID).ToList();
            var erespon = db.EvalutionResponse.Where(r => equestion.Contains(r.DQID)).ToList();
            var gquestion = db.GroupQuestions.Select(g => g.GQID).ToList();
            var grespon = db.GroupERs.Where(g => gquestion.Contains(g.GQID)).ToList();

            svmodel.CID = cid;
            svmodel.CName = db.Courses.Find(cid).CName;
            svmodel.MID = mid;
            svmodel.MName = db.Missions.Find(mid).MName;
            svmodel.gsResponses = gsrespon;
            svmodel.reResponses = rerespon;
            svmodel.eResponses = erespon;
            svmodel.gResponses = grespon;

            if (gsrespon.Any())
            {
                svmodel.IsGoalSetting = true;
            }
            if (rerespon.Any())
            {
                svmodel.IsReflection = true;
            }
            if (erespon.Any())
            {
                svmodel.IsSPEvalution = true;
            }
            if (grespon.Any())
            {
                svmodel.IsGroupEvalution = true;
            }
            svmodel.Students = db.Students.Where(s => s.CID == cid).ToList();

            return View(svmodel);
        }

        public ActionResult StudentGoalSettingDetail(GoalSettingViewModel goalSetting, string cid, string mid, string SID)
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
            goalSetting.SName = db.Students.Find(SID).SName;
            goalSetting.CName = cname;
            goalSetting.MName = mname;

            return View(goalSetting);
        }

        public ActionResult StudentReflectionDetail(GoalSettingViewModel goalSetting, string cid, string mid, string SID)
        {
            var cname = db.Courses.Find(cid).CName;
            var mname = db.Missions.Find(mid).MName;
            var misChat = db.Missions.Find(mid).IsDiscuss;

            goalSetting.IsDiscuss = misChat;
            goalSetting.DefaultQuestions = db.DefaultQuestions.Where(q => q.Class == "自我反思").Include(q => q.Responses);
            goalSetting.MID = mid;
            goalSetting.CID = cid;
            goalSetting.SID = SID;
            goalSetting.SName = db.Students.Find(SID).SName;
            goalSetting.CName = cname;
            goalSetting.MName = mname;

            return View(goalSetting);
        }
    }
}
