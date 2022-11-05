using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using LMSweb.Models;
using LMSweb.ViewModel;

namespace LMSweb.Controllers
{
    public class LearningBehaviorsController : Controller
    {
        private LMSmodel db = new LMSmodel();
        LearnBViewModel vm = new LearnBViewModel();
        public ActionResult StudentJourney(string cid, string mid)//學生普通版學習表現
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).group;
            var sname = db.Students.Find(sid).SName;
            var gqid = 1002.ToString();
            var gid = stuG.GID.ToString();
            var i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Count();

            double TeacherCor = 0;
            double TeacherLogi = 0;
            double TeacherRead = 0;
            double classTeacherCor = 0;
            double classTeacherLogi = 0;
            double classTeacherRead = 0;

            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.SID = sid;
            vm.GID = stuG.GID;
            vm.MID = mid;
            var gid = stuG.GID.ToString();
            vm.SName = sname;
            vm.TeacherER = db.GroupERs.Where(sg => sg.GID == stuG.GID && sg.MID == mid).ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();           
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.CID = cid;
            var i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == stuG.GID && c.MID == mid).ToList().Count();

            if (i != 0)
            {
                TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }

            var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
            vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
            i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
            if (i != 0)
            {
                classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
            vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);

            return View(vm);
        }
        public ActionResult StudentEvalutionJourney(string cid, string mid)//學生互評版學習表現
        {
            ClaimsIdentity claims = (ClaimsIdentity)User.Identity;
            var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();
            var sid = claimData[0].Value;
            var stu = db.Students.Where(s => s.SID == sid);
            var stuG = db.Students.Find(sid).group;
            var sname = db.Students.Find(sid).SName;
            var gqid = 1002.ToString();
            

            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.SID = sid;
            vm.GID = stuG.GID;
            vm.MID = mid;
            var gid = stuG.GID;
            vm.SName = sname;
            vm.TeacherER = db.GroupERs.Where(sg => sg.GID == stuG.GID).ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID != sid && p.MID == mid).ToList();
            vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID  && p.MID == mid).ToList();
            vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList();
            vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList();
            vm.student = db.Students.Where(g => g.group.GID == gid).ToList();
            //小組成果 教師
            vm.CID = cid;
            double TeacherCor = 0;
            double TeacherLogi = 0;
            double TeacherRead = 0;
            double classTeacherCor = 0;
            double classTeacherLogi = 0;
            double classTeacherRead = 0;

            var i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Count();
            if(i != 0)
            {
                TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
            vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
            i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
            if(i != 0)
            {
                classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
            vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
            //小組成果 組間
            double GroupCor = 0;
            double GroupLogi = 0;
            double GroupRead = 0;
            double classGroupCor = 0;
            double classGroupLogi = 0;
            double classGroupRead = 0;

            i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Count();
            if(i != 0)
            {
                GroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                GroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                GroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.GID == stuG.GID && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var GroupScore = ((GroupCor * 0.4) + (GroupLogi * 0.4) + (GroupRead * 0.2));
            vm.GroupScore = Math.Round(GroupScore, 1, MidpointRounding.ToEven);
            i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.MID == mid).ToList().Count();
            if(i != 0)
            {
                classGroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classGroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classGroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
            vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
            vm.GroupComments = db.GroupERs.Where(sg => sg.GQID == 4 && sg.GID == stuG.GID && sg.MID == mid && sg.EvaluatorSID != "T004").ToList();
            //自我評價
            double SelfDiscuss = 0;
            double SelfDraw = 0;
            double SelfCode = 0;
            double SelfContribute = 0;
            double classSelfDiscuss = 0;
            double classSelfDraw = 0;
            double classSelfCode = 0;
            double classSelfContribute = 0;

            i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Count();
            if(i != 0)
            {
                SelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                SelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                SelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                SelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            vm.SelfDiscuss = Math.Round(SelfDiscuss, 1, MidpointRounding.ToEven);
            vm.SelfDraw = Math.Round(SelfDraw, 1, MidpointRounding.ToEven);
            vm.SelfCode = Math.Round(SelfCode, 1, MidpointRounding.ToEven);
            vm.SelfContribute = Math.Round(SelfContribute, 1, MidpointRounding.ToEven);

            i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Count();
            if (i != 0)
            {
                classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));

            }
            vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
            vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
            vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
            vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
            //同儕互評
            double PeerDiscuss = 0;
            double PeerDraw = 0;
            double PeerCode = 0;
            double PeerContribute = 0;
            double classPeerDiscuss = 0;
            double classPeerDraw = 0;
            double classPeerCode = 0;
            double classPeerContribute = 0;

            i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Count();
            if (i != 0)
            {
                PeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                PeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                PeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                PeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));

            }
            vm.PeerDiscuss = Math.Round(PeerDiscuss, 1, MidpointRounding.ToEven);
            vm.PeerDraw = Math.Round(PeerDraw, 1, MidpointRounding.ToEven);
            vm.PeerCode = Math.Round(PeerCode, 1, MidpointRounding.ToEven);
            vm.PeerContribute = Math.Round(PeerContribute, 1, MidpointRounding.ToEven);
            i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Count();
            if(i != 0)
            {
                classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
            vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
            vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
            vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);

            return View(vm);
        }
        public ActionResult TeacherJourney(string cid)//教師學習表現普通版起始頁面
        {
            var gqid = 1002.ToString();
            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.TeacherER = db.GroupERs.ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.CID = cid;
            
            SelectList selectGid = new SelectList(db.Groups.Where(c => c.CID == cid).ToList(), "GID", "GName");
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            ViewBag.SelectList = selectGid;

            double TeacherCor = 0;
            double TeacherLogi = 0;
            double TeacherRead = 0;
            double classTeacherCor = 0;
            double classTeacherLogi = 0;
            double classTeacherRead = 0;

            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.CID = cid;
            var i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID == "T004").ToList().Count();
            if (i != 0)
            {
                TeacherCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                TeacherLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                TeacherRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
            }


            var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
            vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);

            i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID == "T004").ToList().Count();
            if (i != 0)
            {
                classTeacherCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
            }

            var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
            vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
            return View(vm);
        }
        public ActionResult TeacherJourneyR(string cid, string mid, int gid)//教師學習表現普通版輸入條件頁面
        {
            var gqid = 1002.ToString();
            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            var GID = gid.ToString();
            vm.TeacherER = db.GroupERs.ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.CID = cid;
            vm.MID = mid;
            vm.GID = gid;
            double TeacherCor = 0;
            double TeacherLogi = 0;
            double TeacherRead = 0;
            double classTeacherCor = 0;
            double classTeacherLogi = 0;
            double classTeacherRead = 0;

            if (mid == "" && gid == 0)
            {
                return RedirectToAction("TeacherJourney", "LearningBehaviors", new { cid });
            }

            else if (mid == "")
            {
                vm.GName = db.Groups.Find(gid).GName;

                var i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Count();
                if (i != 0)
                {
                    TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                }
                var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);

                i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID == "T004").ToList().Count();
                if (i != 0)
                {
                    classTeacherCor = db.GroupERs.Where(c => c.CID == cid && c.CID == cid && c.GQID == 1 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                    classTeacherLogi = db.GroupERs.Where(c => c.CID == cid && c.CID == cid && c.GQID == 2 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                    classTeacherRead = db.GroupERs.Where(c => c.CID == cid && c.CID == cid && c.GQID == 3 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                }
                var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
            }

            else if(gid == 0)
            {
                vm.MName = db.Missions.Find(mid).MName;

                var i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
                
                var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);

                i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
                if (i != 0)
                {
                    classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                }
                var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
            }

            else if(mid != "" && gid != 0)
            {
                vm.MName = db.Missions.Find(mid).MName;
                vm.GName = db.Groups.Find(gid).GName;

                var i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Count();
                if (i != 0)
                {
                    TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                }
                var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);

                i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
                if (i != 0)
                {
                    classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                }
                var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
            }
            return View(vm);
        }

        public ActionResult TeacherEvalutionJourney(string cid)////教師學習表現互評版起始頁面
        {
            var gqid = 1002.ToString();
            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.TeacherER = db.GroupERs.ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.CID = cid;
            //var Teachercourse = 0;
            //var classcourse = 0;
            ////classcourse = classcourse / db.Groups.Where(c => c.CID == cid).Count();
            //vm.Classcourse = classcourse;
            //vm.Teachercourse = Teachercourse;
            SelectList selectGid = new SelectList(db.Groups.Where(c => c.CID == cid).ToList(), "GID", "GName");
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            ViewBag.SelectList = selectGid;
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid ).ToList();
            vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid ).ToList();
            vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid ).ToList();
            vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList();
            //小組成果 教師
            vm.CID = cid;
            vm.GID = 0;

            double classTeacherCor = 0;
            double classTeacherLogi = 0;
            double classTeacherRead = 0;

            var i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID == "T004").ToList().Count();
           
            if (i != 0)
            {
                classTeacherCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                classTeacherRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
            vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
            //小組成果 組間
            double classGroupCor = 0;
            double classGroupLogi = 0;
            double classGroupRead = 0;

            i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID != "T004").ToList().Count();
            if (i != 0)
            {
                classGroupCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                classGroupLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                classGroupRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
            vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
            vm.GroupComments = db.GroupERs.Where(sg => sg.CID == cid && sg.GQID == 4 && sg.EvaluatorSID != "T004").ToList();
            //自我評價
            double classSelfDiscuss = 0;
            double classSelfDraw = 0;
            double classSelfCode = 0;
            double classSelfContribute = 0;

            i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList().Count();
            if (i != 0)
            {
                classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));

            }
            vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
            vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
            vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
            vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
            //同儕互評
            double classPeerDiscuss = 0;
            double classPeerDraw = 0;
            double classPeerCode = 0;
            double classPeerContribute = 0;

            i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList().Count();
            if (i != 0)
            {
                classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
            }
            vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
            vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
            vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
            vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);
            return View(vm);
        }
        public ActionResult TeacherEvalutionJourneyR(string cid, string mid, int gid, string sid)//教師學習表現互評版輸入條件頁面
        {
            var gqid = 1002.ToString();
            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.TeacherER = db.GroupERs.ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.CID = cid;
            //var Teachercourse = 0;
            //var classcourse = 0;
            ////classcourse = classcourse / db.Groups.Where(c => c.CID == cid).Count();
            //vm.Classcourse = classcourse;
            //vm.Teachercourse = Teachercourse;
            SelectList selectGid = new SelectList(db.Groups.Where(c => c.CID == cid).ToList(), "GID", "GName");
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            ViewBag.SelectList = selectGid;
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.Groups = db.Groups.Where(g => g.CID == cid).ToList();
            vm.ClassER = db.GroupERs.ToList();
            vm.Teachercourse = db.Groups.Where(c => c.CID == cid).Count();
            vm.missions = db.Missions.Where(m => m.CID == cid).ToList();
            vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid).ToList();
            vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid).ToList();
            vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid).ToList();
            vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList();
            vm.MID = mid;
            vm.GID = gid;
            vm.SID = sid;
            double TeacherCor = 0;
            double TeacherLogi = 0;
            double TeacherRead = 0;
            double classTeacherCor = 0;
            double classTeacherLogi = 0;
            double classTeacherRead = 0;

            double GroupCor = 0;
            double GroupLogi = 0;
            double GroupRead = 0;
            double classGroupCor = 0;
            double classGroupLogi = 0;
            double classGroupRead = 0;

            double SelfDiscuss = 0;
            double SelfDraw = 0;
            double SelfCode = 0;
            double SelfContribute = 0;
            double classSelfDiscuss = 0;
            double classSelfDraw = 0;
            double classSelfCode = 0;
            double classSelfContribute = 0;

            double PeerDiscuss = 0;
            double PeerDraw = 0;
            double PeerCode = 0;
            double PeerContribute = 0;
            double classPeerDiscuss = 0;
            double classPeerDraw = 0;
            double classPeerCode = 0;
            double classPeerContribute = 0;

            if(mid == "")
            {
                if(gid != 0)
                {
                    vm.GName = db.Groups.Find(gid).GName;
                    if(sid == "")//NYN
                    {
                        vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList();
                        vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList();
                        vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList();
                        vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList();
                        vm.student = db.Students.Where(g => g.group.GID == gid).ToList();

                        //小組成果 教師
                        var i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid).ToList().Count();
                        if (i != 0)
                        {
                            TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                        vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001")).ToList().Count();
                        if (i != 0)
                        {
                            classTeacherCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                        vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
                        //小組成果 組間

                        i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.GID == gid).ToList().Count();
                        if (i != 0)
                        {
                            GroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var GroupScore = ((GroupCor * 0.4) + (GroupLogi * 0.4) + (GroupRead * 0.2));
                        vm.GroupScore = Math.Round(GroupScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID != "T004").ToList().Count();
                        if (i != 0)
                        {
                            classGroupCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
                        vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
                        vm.GroupComments = db.GroupERs.Where(sg => sg.GQID == 4 && sg.GID == gid && sg.EvaluatorSID != "T004").ToList();
                        //自我評價

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList().Count();
                        if (i != 0)
                        {
                            SelfDiscuss = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 1 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfDraw = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 2 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfCode = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 3 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfContribute = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 4 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.SelfDiscuss = Math.Round(SelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.SelfDraw = Math.Round(SelfDraw, 1, MidpointRounding.ToEven);
                        vm.SelfCode = Math.Round(SelfCode, 1, MidpointRounding.ToEven);
                        vm.SelfContribute = Math.Round(SelfContribute, 1, MidpointRounding.ToEven);

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList().Count();
                        if (i != 0)
                        {
                            classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
                        vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
                        vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
                        //同儕互評

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList().Count();
                        if (i != 0)
                        {
                            PeerDiscuss = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 1 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerDraw = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 2 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerCode = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 3 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerContribute = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 4 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.PeerDiscuss = Math.Round(PeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.PeerDraw = Math.Round(PeerDraw, 1, MidpointRounding.ToEven);
                        vm.PeerCode = Math.Round(PeerCode, 1, MidpointRounding.ToEven);
                        vm.PeerContribute = Math.Round(PeerContribute, 1, MidpointRounding.ToEven);
                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList().Count();
                        if (i != 0)
                        {
                            classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
                        vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
                        vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);
                    }
                    else if(sid != "")//NYY
                    {
                        vm.SName = db.Students.Find(sid).SName;
                        vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID != sid).ToList();
                        vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList();
                        vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID == sid ).ToList();
                        vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList();
                        vm.student = db.Students.Where(g => g.group.GID == gid).ToList();

                        //小組成果 教師
                        var i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid).ToList().Count();
                        if (i != 0)
                        {
                            TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                        vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID == "T004").ToList().Count();
                        if (i != 0)
                        {
                            classTeacherCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID == "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                        vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
                        //小組成果 組間

                        i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.GID == gid).ToList().Count();
                        if (i != 0)
                        {
                            GroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.GID == gid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var GroupScore = ((GroupCor * 0.4) + (GroupLogi * 0.4) + (GroupRead * 0.2));
                        vm.GroupScore = Math.Round(GroupScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => c.CID == cid && c.EvaluatorSID != "T004").ToList().Count();
                        if (i != 0)
                        {
                            classGroupCor = db.GroupERs.Where(c => c.CID == cid && c.GQID == 1 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupLogi = db.GroupERs.Where(c => c.CID == cid && c.GQID == 2 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupRead = db.GroupERs.Where(c => c.CID == cid && c.GQID == 3 && c.EvaluatorSID != "T004").ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
                        vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
                        vm.GroupComments = db.GroupERs.Where(sg => sg.GQID == 4 && sg.GID == gid && sg.EvaluatorSID != "T004").ToList();
                        //自我評價

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID == sid).ToList().Count();
                        if (i != 0)
                        {
                            SelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.SelfDiscuss = Math.Round(SelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.SelfDraw = Math.Round(SelfDraw, 1, MidpointRounding.ToEven);
                        vm.SelfCode = Math.Round(SelfCode, 1, MidpointRounding.ToEven);
                        vm.SelfContribute = Math.Round(SelfContribute, 1, MidpointRounding.ToEven);

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID).ToList().Count();
                        if (i != 0)
                        {
                            classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
                        vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
                        vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
                        //同儕互評

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != sid && p.EvaluatorSID == sid).ToList().Count();
                        if (i != 0)
                        {
                            PeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != sid && p.EvaluatorSID == sid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.PeerDiscuss = Math.Round(PeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.PeerDraw = Math.Round(PeerDraw, 1, MidpointRounding.ToEven);
                        vm.PeerCode = Math.Round(PeerCode, 1, MidpointRounding.ToEven);
                        vm.PeerContribute = Math.Round(PeerContribute, 1, MidpointRounding.ToEven);
                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID).ToList().Count();
                        if (i != 0)
                        {
                            classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
                        vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
                        vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);
                    }
                }
                if(gid == 0)//NNN
                {
                    return RedirectToAction("TeacherEvalutionJourney", "LearningBehaviors", new { cid });
                }
            }
            else if(mid != "")
            {
                vm.MName = db.Missions.Find(mid).MName;

                if (gid != 0)
                {
                    vm.GName = db.Groups.Find(gid).GName;
                    if (sid == "")//YYN
                    {
                        
                        vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList();
                        vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList();
                        vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList();
                        vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList();
                        vm.student = db.Students.Where(g => g.group.GID == gid).ToList();

                        //小組成果 教師
                        var i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                        vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                        vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
                        //小組成果 組間

                        i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.GID == gid && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            GroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var GroupScore = ((GroupCor * 0.4) + (GroupLogi * 0.4) + (GroupRead * 0.2));
                        vm.GroupScore = Math.Round(GroupScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classGroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
                        vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
                        vm.GroupComments = db.GroupERs.Where(sg => sg.GQID == 4 && sg.GID == gid && sg.MID == mid && sg.EvaluatorSID != "T004" && sg.EvaluatorSID != "T001").ToList();
                        //自我評價

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            SelfDiscuss = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 1 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfDraw = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 2 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfCode = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 3 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfContribute = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 4 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.SelfDiscuss = Math.Round(SelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.SelfDraw = Math.Round(SelfDraw, 1, MidpointRounding.ToEven);
                        vm.SelfCode = Math.Round(SelfCode, 1, MidpointRounding.ToEven);
                        vm.SelfContribute = Math.Round(SelfContribute, 1, MidpointRounding.ToEven);

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
                        vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
                        vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
                        //同儕互評

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            PeerDiscuss = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 1 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerDraw = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 2 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerCode = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 3 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerContribute = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 4 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.PeerDiscuss = Math.Round(PeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.PeerDraw = Math.Round(PeerDraw, 1, MidpointRounding.ToEven);
                        vm.PeerCode = Math.Round(PeerCode, 1, MidpointRounding.ToEven);
                        vm.PeerContribute = Math.Round(PeerContribute, 1, MidpointRounding.ToEven);
                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
                        vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
                        vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);
                    }
                    else if (sid != "")//YYY
                    {
                        vm.SName = db.Students.Find(sid).SName;
                        vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID != sid && p.MID == mid).ToList();
                        vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList();
                        vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList();
                        vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList();
                        vm.student = db.Students.Where(g => g.group.GID == gid).ToList();

                        //小組成果 教師
                        var i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            TeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            TeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                        vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                        vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
                        //小組成果 組間

                        i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.GID == gid && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            GroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            GroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.GID == gid && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var GroupScore = ((GroupCor * 0.4) + (GroupLogi * 0.4) + (GroupRead * 0.2));
                        vm.GroupScore = Math.Round(GroupScore, 1, MidpointRounding.ToEven);
                        i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classGroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classGroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
                        vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
                        vm.GroupComments = db.GroupERs.Where(sg => sg.GQID == 4 && sg.GID == gid && sg.MID == mid && sg.EvaluatorSID != "T004").ToList();
                        //自我評價

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            SelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            SelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.SelfDiscuss = Math.Round(SelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.SelfDraw = Math.Round(SelfDraw, 1, MidpointRounding.ToEven);
                        vm.SelfCode = Math.Round(SelfCode, 1, MidpointRounding.ToEven);
                        vm.SelfContribute = Math.Round(SelfContribute, 1, MidpointRounding.ToEven);

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
                        vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
                        vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
                        vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
                        //同儕互評

                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            PeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            PeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != sid && p.EvaluatorSID == sid && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.PeerDiscuss = Math.Round(PeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.PeerDraw = Math.Round(PeerDraw, 1, MidpointRounding.ToEven);
                        vm.PeerCode = Math.Round(PeerCode, 1, MidpointRounding.ToEven);
                        vm.PeerContribute = Math.Round(PeerContribute, 1, MidpointRounding.ToEven);
                        i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Count();
                        if (i != 0)
                        {
                            classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                            classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        }
                        vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
                        vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
                        vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
                        vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);
                    }
                }
                if (gid == 0)//YNN
                {
                    vm.PeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList();
                    vm.GPeerER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList();
                    vm.SelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList();
                    vm.GSelfER = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList();
                    //vm.student = db.Students.Where(g => g.group.GID == gid).ToList();

                    //小組成果 教師
                    var i = db.GroupERs.Where(c => (c.EvaluatorSID == "T004") && c.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        TeacherCor = 0;//db.GroupERs.Where(c => c.GQID == 1 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        TeacherLogi = 0;//db.GroupERs.Where(c => c.GQID == 2 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        TeacherRead = 0;//db.GroupERs.Where(c => c.GQID == 3 && (c.EvaluatorSID == "T004" || c.EvaluatorSID == "T001") && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    var TeacherScore = ((TeacherCor * 0.4) + (TeacherLogi * 0.4) + (TeacherRead * 0.2));
                    vm.TeacherScore = Math.Round(TeacherScore, 1, MidpointRounding.ToEven);
                    i = db.GroupERs.Where(c => c.EvaluatorSID == "T004" && c.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        classTeacherCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classTeacherLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classTeacherRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID == "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    var classTeacherScore = ((classTeacherCor * 0.4) + (classTeacherLogi * 0.4) + (classTeacherRead * 0.2));
                    vm.classTeacherScore = Math.Round(classTeacherScore, 1, MidpointRounding.ToEven);
                    //小組成果 組間

                    i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        GroupCor = 0;//db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        GroupLogi = 0;//db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        GroupRead = 0;//db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.EvaluatorSID != "T001" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    var GroupScore = ((GroupCor * 0.4) + (GroupLogi * 0.4) + (GroupRead * 0.2));
                    vm.GroupScore = Math.Round(GroupScore, 1, MidpointRounding.ToEven);
                    i = db.GroupERs.Where(c => c.EvaluatorSID != "T004" && c.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        classGroupCor = db.GroupERs.Where(c => c.GQID == 1 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classGroupLogi = db.GroupERs.Where(c => c.GQID == 2 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classGroupRead = db.GroupERs.Where(c => c.GQID == 3 && c.EvaluatorSID != "T004" && c.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    var classGroupScore = ((classGroupCor * 0.4) + (classGroupLogi * 0.4) + (classGroupRead * 0.2));
                    vm.classGroupScore = Math.Round(classGroupScore, 1, MidpointRounding.ToEven);
                    vm.GroupComments = db.GroupERs.Where(sg => sg.GQID == 4 && sg.MID == mid && sg.EvaluatorSID != "T004").ToList();
                    //自我評價

                    i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        SelfDiscuss = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 1 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        SelfDraw = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 2 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        SelfCode = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 3 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        SelfContribute = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 4 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    vm.SelfDiscuss = Math.Round(SelfDiscuss, 1, MidpointRounding.ToEven);
                    vm.SelfDraw = Math.Round(SelfDraw, 1, MidpointRounding.ToEven);
                    vm.SelfCode = Math.Round(SelfCode, 1, MidpointRounding.ToEven);
                    vm.SelfContribute = Math.Round(SelfContribute, 1, MidpointRounding.ToEven);

                    i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        classSelfDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classSelfDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classSelfCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classSelfContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID == p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    vm.classSelfDiscuss = Math.Round(classSelfDiscuss, 1, MidpointRounding.ToEven);
                    vm.classSelfDraw = Math.Round(classSelfDraw, 1, MidpointRounding.ToEven);
                    vm.classSelfCode = Math.Round(classSelfCode, 1, MidpointRounding.ToEven);
                    vm.classSelfContribute = Math.Round(classSelfContribute, 1, MidpointRounding.ToEven);
                    //同儕互評

                    i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        PeerDiscuss = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 1 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        PeerDraw = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 2 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        PeerCode = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 3 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        PeerContribute = 0;//db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 4 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    vm.PeerDiscuss = Math.Round(PeerDiscuss, 1, MidpointRounding.ToEven);
                    vm.PeerDraw = Math.Round(PeerDraw, 1, MidpointRounding.ToEven);
                    vm.PeerCode = Math.Round(PeerCode, 1, MidpointRounding.ToEven);
                    vm.PeerContribute = Math.Round(PeerContribute, 1, MidpointRounding.ToEven);
                    i = db.EvalutionResponse.Where(p => p.CID == cid && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Count();
                    if (i != 0)
                    {
                        classPeerDiscuss = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 20 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classPeerDraw = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 21 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classPeerCode = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 22 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                        classPeerContribute = db.EvalutionResponse.Where(p => p.CID == cid && p.DQID == 23 && p.SID != p.EvaluatorSID && p.MID == mid).ToList().Average(t => Convert.ToInt32(t.Answer));
                    }
                    vm.classPeerDiscuss = Math.Round(classPeerDiscuss, 1, MidpointRounding.ToEven);
                    vm.classPeerDraw = Math.Round(classPeerDraw, 1, MidpointRounding.ToEven);
                    vm.classPeerCode = Math.Round(classPeerCode, 1, MidpointRounding.ToEven);
                    vm.classPeerContribute = Math.Round(classPeerContribute, 1, MidpointRounding.ToEven);
                }
            }
            
            return View(vm);
        }
        public ActionResult PersonalJourney(string CID)
        {
            //var results = db.LearnB.Where(j => j.StudentMissions.SID == "S001");
            //ViewBag.Studemt = db.LearnB.Where(s => s.StudentMissions.SID == "S001");
            //產生ViewModel物件
            vm.CID = CID;
           // vm.learningbehavior = db.LearnB.Where(s => s.StudentMissions.SID == "S001").ToList();(因修改Model所以註解)
            return View(vm);
        }
        public ActionResult Chat()
        {         
            return View();
        }
        [HttpPost]
        public JsonResult Studsnts(int gid)
        {
            var GID = gid.ToString();
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(GID))
            {
                var orders = this.GetStu(gid);
                if (orders.Count() > 0)
                {
                    foreach (var order in orders)
                    {
                        items.Add(new KeyValuePair<string, string>(
                            order.SID,
                            string.Format("{0}", order.SName)));
                    }
                }
            }
            return this.Json(items);
        }
        private IEnumerable<Student> GetStu(int gid)
        {
            var GID = gid.ToString();
            using (LMSmodel db = new LMSmodel())
            {
                var query = db.Students.Where(x => x.group.GID == gid).OrderBy(x => x.SName);
                return query.ToList();
            }
        }
    }
}
