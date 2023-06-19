using LMSweb.Models;
using LMSweb.ViewModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.Assets
{
    public class GuideForStudent
    {

        private string mid;
        private string cid;
        private string sid;

        private LMSmodel db;

        private int TestType;
        private Mission MissionData;
        private List<Student> Leaders;

        private DateTime StartDate;
        private DateTime EndDate;

        public GuideForStudent(string mid, string cid)
        {
            this.mid = mid;
            this.cid = cid;

            this.db = new LMSmodel();

            this.TestType = db.Courses.Find(cid).TestType;
            this.MissionData = db.Missions.Find(mid);

            this.Leaders = (from e in db.Executions
                            join s in db.Students on e.GID equals s.GID
                            join g in db.Groups on e.GID equals g.GID
                            where e.MID == mid && s.IsLeader == true
                            select s).ToList();

            this.StartDate = DateTime.Parse(MissionData.Start);
            this.EndDate = DateTime.Parse(MissionData.End);
        }
        public GuideForStudent(string mid, string cid, string sid) : this(mid, cid)
        {
            this.sid = sid;
        }
        // 幫我創建一個判斷目前步驟進度方法
        public void UpdateCurrentStatus()
        {
            switch (TestType)
            {
                case 0:
                    UpdateCurrentStatusForTestType0();
                    break;
                case 1:
                    UpdateCurrentStatusForTestType1();
                    break;
                case 2:
                    UpdateCurrentStatusForTestType2();
                    break;
                case 3:
                    UpdateCurrentStatusForTestType3();
                    break;
                case 4:
                    UpdateCurrentStatusForTestType4();
                    break;
                case 5:
                    UpdateCurrentStatusForTestType5();
                    break;
                default:
                    break;
            }
        }

        private void UpdateCurrentStatusForTestType0()
        {
            if (sid != null)
            {
                Student student;
                Execution execution;
                SetExecutionByCurrentStatus(out student, out execution);

                // 開啟第一步驟畫流程圖
                if (execution.CurrentStatus == "000")
                {
                    execution.CurrentStatus = "100";
                }

                // 判斷流程圖是否上傳
                else if (execution.CurrentStatus == "100")
                {
                    var Draw = db.StudentDraws.Where(x => x.GID == student.GID && x.MID == mid).FirstOrDefault();

                    if (Draw != null)
                    {
                        execution.CurrentStatus = "210";
                    }
                }

                // 判斷程式碼是否上傳
                else if (execution.CurrentStatus == "210")
                {
                    var Code = db.StudentCodes.Where(x => x.GID == student.GID && x.MID == mid).FirstOrDefault();

                    if (Code != null)
                    {
                        execution.CurrentStatus = "221";
                    }
                }
                db.SaveChanges();
            }
        }

        private void UpdateCurrentStatusForTestType1()
        {
            if (sid != null)
            {
                Student student;
                Execution execution;
                SetExecutionByCurrentStatus(out student, out execution);

                // 開啟第一步驟畫流程圖
                if (execution.CurrentStatus == "000000")
                {
                    execution.CurrentStatus = "100000";
                }

                // 判斷流程圖是否上傳
                else if (execution.CurrentStatus == "100000")
                {
                    var Draw = db.StudentDraws.Where(x => x.GID == student.GID && x.MID == mid).FirstOrDefault();

                    if (Draw != null)
                    {
                        execution.CurrentStatus = "210000";
                    }
                }

                // 判斷程式碼是否上傳
                else if (execution.CurrentStatus == "210000")
                {
                    var Code = db.StudentCodes.Where(x => x.GID == student.GID && x.MID == mid).FirstOrDefault();

                    if (Code != null)
                    {
                        execution.CurrentStatus = "221000";
                    }
                }
                db.SaveChanges();
            }
        }

        private void UpdateCurrentStatusForTestType2()
        {
            if (sid != null)
            {
                Student student;
                Execution execution;
                SetExecutionByCurrentStatus(out student, out execution);
            }
        }

        private void UpdateCurrentStatusForTestType3()
        {
            if (sid != null)
            {
                Student student;
                Execution execution;
                SetExecutionByCurrentStatus(out student, out execution);
            }
        }

        private void UpdateCurrentStatusForTestType4()
        {
            if (sid != null)
            {
                Student student;
                Execution execution;
                SetExecutionByCurrentStatus(out student, out execution);
            }
        }

        private void UpdateCurrentStatusForTestType5()
        {
            if (sid != null)
            {
                Student student;
                Execution execution;
                SetExecutionByCurrentStatus(out student, out execution);
            }
        }

        // 這邊要做的事情是，把所有學生的CurrentStatus都設為00，代表任務尚未開始，做初始化。
        private void SetExecutionByCurrentStatus(out Student student, out Execution execution)
        {
            student = db.Students.Find(sid);
            execution = db.Executions.Find(student.GID, mid);
            if (execution == null)
            {
                var _execution = new Execution()
                {
                    GID = student.GID,
                    MID = mid,
                    CurrentStatus = GlobalClass.DefaultCurrentStatus(TestType),
                };
                db.Executions.Add(_execution);
                db.SaveChanges();
                execution = db.Executions.Find(student.GID, mid);
            }
        }
    }
}