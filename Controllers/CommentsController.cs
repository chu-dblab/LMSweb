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
using LMSweb.ViewModel.GroupAssessment;

namespace LMSweb.Controllers
{
    public class CommentController : Controller
    {
        private LMSmodel db = new LMSmodel();

        public ActionResult Index(string commentid)
        {
            var comments = db.Comments.Where(x => x.CommentID == commentid);
            return View();
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        public ActionResult Create()
        {
            ViewBag.CommentTypes = new SelectList(db.CommentTypes, "CommentTypeID", "CTContent");
            ViewBag.Students = new SelectList(db.Students, "SID", "SName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string commentTypeID, string cContent)
        {
            if (!string.IsNullOrEmpty(commentTypeID) && !string.IsNullOrEmpty(cContent))
            {
                Comment comment = new Comment
                {
                    CommentID = Guid.NewGuid().ToString(),
                    CContent = cContent,
                    CommentTypes = db.CommentTypes.Find(commentTypeID),
                };

                db.Comments.Add(comment);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CommentTypes = new SelectList(db.CommentTypes, "CommentTypeID", "CTContent");
            ViewBag.Students = new SelectList(db.Students, "SID", "SName");
            return View();
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            ViewBag.CommentTypes = new SelectList(db.CommentTypes, "CommentTypeID", "CTContent", comment.CommentTypes.CommentTypeID);
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string commentID, string commentTypeID, string cContent)
        {
            if (!string.IsNullOrEmpty(commentID) && !string.IsNullOrEmpty(commentTypeID) && !string.IsNullOrEmpty(cContent))
            {
                Comment comment = db.Comments.Find(commentID);

                if (comment == null)
                {
                    return HttpNotFound();
                }

                comment.CommentTypes = db.CommentTypes.Find(commentTypeID);
                comment.CContent = cContent;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CommentTypes = new SelectList(db.CommentTypes, "CommentTypeID", "CTContent");
            return View();
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Comment comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            db.Comments.Remove(comment);
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
    }
}
