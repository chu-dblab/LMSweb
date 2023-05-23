using LMSweb.Models;
using LMSweb.ViewModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.Assets
{
    public class Guide
    {
        // string mid,string cid
        private string mid;
        private string cid;
        private int TestType;
        //private string StartTime;
        //private string EndTime;
        //private string CurrentAction;

        private LMSmodel db = new LMSmodel();

        public Guide(string mid, string cid)
        {
            this.mid = mid;
            this.cid = cid;

            this.TestType = (from m in db.Missions
                             from c in db.Courses
                             where m.CID == c.CID && m.MID == mid && c.CID == cid
                             select c.TestType
                        ).FirstOrDefault();
        }

        // 幫我創建一個判斷目前步驟進度方法
        public string GetNewCurrentAction()
        {
            switch (TestType)
            {
                case 0:
                    return GetNewCurrentActionForTestType0();
                case 1:
                    return GetNewCurrentActionForTestType1();
                case 2:
                    return GetNewCurrentActionForTestType2();
                case 3:
                    return GetNewCurrentActionForTestType3();
                case 4:
                    return GetNewCurrentActionForTestType4();
                case 5:
                    return GetNewCurrentActionForTestType5();
                default:
                    return "";
            }
        }

        private string GetNewCurrentActionForTestType0()
        {
            var MissionsData = (from m in db.Missions
                                join c in db.Courses on m.CID equals c.CID
                                where m.MID == mid && c.CID == cid
                                select new
                                {
                                    m.Start,
                                    m.End,
                                    m.CurrentAction,
                                    m.IsAssess,
                                }).FirstOrDefault();
            if (MissionsData != null)
            {
                var StartDate = DateTime.Parse(MissionsData.Start);
                var EndDate = DateTime.Parse(MissionsData.End);

                // 啟動任務引導系統
                if (MissionsData.CurrentAction == "00")
                {
                    if (DateTime.Now < StartDate)
                    {
                        return "00";
                    }
                    else
                    {
                        return "10";
                    }
                }
                //if (MissionsData.CurrentAction == "10")
                //{
                //    var CourseData = (from c in db.Courses
                //                      from m in db.Missions
                //                      from 
                //                      where c.CID == m.CID && c.CID == cid && m.MID == mid
                //}
            }


            throw new NotImplementedException();
        }
        private string GetNewCurrentActionForTestType1()
        {
            throw new NotImplementedException();
        }
        private string GetNewCurrentActionForTestType2()
        {
            throw new NotImplementedException();
        }
        private string GetNewCurrentActionForTestType3()
        {
            throw new NotImplementedException();
        }
        private string GetNewCurrentActionForTestType4()
        {
            throw new NotImplementedException();
        }
        private string GetNewCurrentActionForTestType5()
        {
            throw new NotImplementedException();
        }
    }
}