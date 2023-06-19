using LMSweb.Models;
using LMSweb.Assets;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.VariantTypes;

namespace LMSweb.Service
{
    public class QuestionService
    {
        private readonly LMSmodel db;

        public QuestionService()
        {
            db = new LMSmodel();
        }

        public void AddDefaultQuestionToMission(int DefaultQuestionsID, string mid, GlobalClass.TaskSteps TaskStepsString)
        {
            // #################### 測試用資料 ###############################
            TaskStepsString = GlobalClass.TaskSteps.TeamReflection;
            // ###############################################################


            DefaultQuestion defaultQuestion = db.DefaultQuestions.Find(DefaultQuestionsID); //default

            var _questionNew = new QuestionNew()
            {
                QuestionNewID = "Q" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                QContent = defaultQuestion.Description,
                QType = defaultQuestion.Type,
                CID = db.Missions.Find(mid).CID,
                EProcedureID = $"{TaskStepsString}"
            };

            db.QuestionNews.Add(_questionNew);
            db.SaveChanges();

            // 將選擇題的選項加入資料庫
            if (!(defaultQuestion.Type == GlobalClass.QuestionType.EssayQuestion.ToString()))
            {
                // 找出剛剛新增的題目的選項
                var defaultOptions = db.DefaultOptions.Where(o => o.DQID == DefaultQuestionsID).ToList();
                foreach (var d_option in defaultOptions)
                {
                    var _optionNew = new OptionNew()
                    {
                        OContent = d_option.DOptions,
                        QuestionNewID = _questionNew.QuestionNewID,
                        QuestionNew = _questionNew,
                    };
                }
                db.SaveChanges();
            }
        }
    }
}