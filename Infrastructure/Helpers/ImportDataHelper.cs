using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using LMSweb.Models;
using LinqToExcel;

namespace LMSweb.Infrastructure.Helpers
{
    public class ImportDataHelper
    {
        /// <summary>
        /// 檢查匯入的 Excel 資料.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="importStudent">The import zip codes.</param>
        /// <returns></returns>
        public CheckResult CheckImportData(string fileName, List<Student> importStudent)
        {
            var result = new CheckResult();

            var targetFile = new FileInfo(fileName);


            if (!targetFile.Exists)
            {
                result.ID = Guid.NewGuid();
                result.Success = false;
                result.ErrorCount = 0;
                result.ErrorMessage = "匯入的資料檔案不存在";
                return result;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            //欄位對映
            excelFile.AddMapping<Student>(x => x.SID, "學號");
            excelFile.AddMapping<Student>(x => x.SName, "學生姓名");
            excelFile.AddMapping<Student>(x => x.SPassword, "密碼");
            excelFile.AddMapping<Student>(x => x.Sex, "性別");
            //excelFile.AddMapping<Student>(x => x.Score, "分數");
            //excelFile.AddMapping<Student>(x => x.CourseID, "CourseID");

            //SheetName
            var excelContent = excelFile.Worksheet<Student>("工作表1").Where(s =>s.SID != "");

            int errorCount = 0;
            int rowIndex = 1;
            var importErrorMessages = new List<string>();

            //檢查資料
            using (var db = new LMSmodel())
            {
                var db_students = db.Students;
                foreach (var row in excelContent)
                {
                    var errorMessage = new StringBuilder();
                    var student = new Student();

                    student.SID = row.SID;
                    student.SName = row.SName;
                    //student.SPassword = row.SPassword;
                   student.Sex = row.Sex;
                    //student.Score = row.Score;
                    //student.CourseID = row.CourseID;


                    if (string.IsNullOrWhiteSpace(row.SID))
                    {
                        errorMessage.Append("學生ID - 不可空白. ");
                    }
                    
                    if (db_students.Find(row.SID) != null)
                    {
                        errorMessage.Append("學生ID - 已重複. ");
                    }
                    student.SID = row.SID;

                    if (string.IsNullOrWhiteSpace(row.SName))
                    {
                        errorMessage.Append("學生姓名 - 不可空白. ");
                    }
                    student.SName = row.SName;


                    //if (string.IsNullOrWhiteSpace(row.SPassword))
                    //{
                    //    errorMessage.Append("學生密碼 - 不可空白. ");
                    //}
                    student.SPassword = row.SID;

                    //=============================================================================
                    if (errorMessage.Length > 0)
                    {
                        errorCount += 1;
                        importErrorMessages.Add(string.Format(
                            "第 {0} 列資料發現錯誤：{1}{2}",
                            rowIndex,
                            errorMessage,
                            "<br/>"));
                    }
                    else
                    {
                       // student.SPassword = row.SID;
                        importStudent.Add(student);
                    }
                    rowIndex += 1;
                }
            }

            try
            {
                result.ID = Guid.NewGuid();
                result.Success = errorCount.Equals(0);
                result.RowCount = importStudent.Count;
                result.ErrorCount = errorCount;

                string allErrorMessage = string.Empty;

                foreach (var message in importErrorMessages)
                {
                    allErrorMessage += message;
                }

                result.ErrorMessage = allErrorMessage;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Saves the import data.
        /// </summary>
        /// <param name="importZipCodes">The import zip codes.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SaveImportData(IEnumerable<Student> importStudent, string cid)
        {
            try
            {
                //先砍掉全部資料
                //using (var db = new LMSmodel())
                //{
                //    foreach (var item in db.Students.OrderBy(x => x.ID))
                //    {
                //        db.Students.Remove(item);
                //    }
                //    db.SaveChanges();
                //}

                //再把匯入的資料給存到資料庫
                using (var db = new LMSmodel())
                {
                    foreach (var item in importStudent)
                    {
                        // GID = 6 代表學生尚未分組
                        item.CID = cid;
                        item.IsLeader = false;
                        item.GID = 6;
                        db.Students.Add(item);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}