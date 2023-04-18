using LMSweb.Models;
using LMSweb.Service;
using LMSweb.ViewModel;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace LMSweb
{
    public class CodeServiceHub : Hub
    {
        private LMSmodel db = new LMSmodel();
        private TextIO textIO = new TextIO();

        public void editCode(string gid, string content, int line, int ch)
        {
            //Clients.All.broadcastCode(content, line, ch);
            Clients.Group(gid).broadcastCode(content, line, ch);
        }

        public void saveCode(StudentCodingViewModel viewModel)
        {
            StudentCode model_insert = null;
            string FileName = viewModel.CID + " " + viewModel.MID + " " + viewModel.GID;

            StudentCode model_update = db.StudentCodes.Find(viewModel.CID, viewModel.MID, viewModel.GID);
            if(model_update == null)
            {
                model_insert = new StudentCode();
                model_insert.CID = viewModel.CID;
                model_insert.MID = viewModel.MID;
                model_insert.GID = viewModel.GID;
                model_insert.IsEdit = viewModel.IsEdit;
                //model_insert.course = db.Courses.Find(viewModel.CourseID);
                //model_insert.mission = db.Missions.Find(viewModel.MID);
                //model_insert.group = db.Groups.Find(viewModel.GID);
                model_insert.CodePath = FileName;
                db.StudentCodes.Add(model_insert);
            }
            else
            {
                model_update.CID = viewModel.CID;
                model_update.MID = viewModel.MID;
                model_update.GID = viewModel.GID;
                model_update.IsEdit = viewModel.IsEdit;
                //model_update.course = db.Courses.Find(viewModel.CourseID);
                //model_update.mission = db.Missions.Find(viewModel.MID);
                //model_update.group = db.Groups.Find(viewModel.GID);
                model_update.CodePath = FileName;
            }

            db.SaveChanges();

            textIO.SaveFile(FileName, viewModel.CodeContent);
        }

        public async Task readCode(string cid, string mid, string gid)
        {
            string discPath = WebConfigurationManager.AppSettings["CodePath"];
            string PathRoot = HttpContext.Current.Server.MapPath(discPath);
            string FileName = cid + " " + mid + " " + gid;
            string content = textIO.readCodeText(PathRoot + FileName + ".txt");
            await joinGroup(gid);
            //Clients.All.broadcastCode(content, 0, 0);
            Clients.Group(gid).broadcastCode(content, 0, 0);
        }
        public Task joinGroup(string gid)
        {
            return Groups.Add(Context.ConnectionId, gid);
        }
        public Task removeGroup(string gid)
        {
            return Groups.Remove(Context.ConnectionId, gid);
        }
    }
}