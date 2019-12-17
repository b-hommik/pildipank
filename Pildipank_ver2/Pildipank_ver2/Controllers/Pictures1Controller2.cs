using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pildipank_ver2.Models;

namespace Pildipank_ver2.Controllers
{
    public class Pictures1Controller2 : Controller
    {
        private ParemEsimeneEntities db = new ParemEsimeneEntities();

        // GET: Pictures1Controller2
        public ActionResult Index()
        {
            var pictures = db.Pictures.Include(p => p.Album).Include(p => p.AspNetUser).Include(p => p.DataFile).Include(p => p.Category);
            return View(pictures.ToList());
        }

        // GET: Pictures1Controller2/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // GET: Pictures1Controller2/Create
        public ActionResult Create()
        {
            ViewBag.AlbumId = new SelectList(db.Albums, "Id", "UserId");
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.FileId = new SelectList(db.DataFiles, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Pictures1Controller2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,FileId,Name,Description,Created,Visibility,AlbumId,CategoryId")] Picture picture)
        {
            if (ModelState.IsValid)
            {
                db.Pictures.Add(picture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AlbumId = new SelectList(db.Albums, "Id", "UserId", picture.AlbumId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", picture.UserId);
            ViewBag.FileId = new SelectList(db.DataFiles, "Id", "Name", picture.FileId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", picture.CategoryId);
            return View(picture);
        }

        // GET: Pictures1Controller2/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlbumId = new SelectList(db.Albums, "Id", "UserId", picture.AlbumId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", picture.UserId);
            ViewBag.FileId = new SelectList(db.DataFiles, "Id", "Name", picture.FileId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", picture.CategoryId);
            return View(picture);
        }

        // POST: Pictures1Controller2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,FileId,Name,Description,Created,Visibility,AlbumId,CategoryId")] Picture picture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(picture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AlbumId = new SelectList(db.Albums, "Id", "UserId", picture.AlbumId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", picture.UserId);
            ViewBag.FileId = new SelectList(db.DataFiles, "Id", "Name", picture.FileId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", picture.CategoryId);
            return View(picture);
        }

        // GET: Pictures1Controller2/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            return View(picture);
        }

        // POST: Pictures1Controller2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Picture picture = db.Pictures.Find(id);
            db.Pictures.Remove(picture);
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
