using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pildipank_ver2.Models;

namespace Pildipank_ver2.Controllers
{
    public class DataFilesController : Controller
    {
        private ParemEsimeneEntities db = new ParemEsimeneEntities();

        [OutputCache(VaryByParam = "id", Location = System.Web.UI.OutputCacheLocation.ServerAndClient, Duration = 3600)]
        public ActionResult Content(int id)
        {
            DataFile file = db.DataFiles.Find(id);
            if (file == null) return HttpNotFound();

            return File(file.Content, file.ContentType);
        }

        // GET: DataFiles
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.DataFiles.ToList());
        }

        // GET: DataFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataFile dataFile = db.DataFiles.Find(id);
            if (dataFile == null)
            {
                return HttpNotFound();
            }
            return View(dataFile);
        }

        // GET: DataFiles/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
  
        {
            return View();
        }

        // POST: DataFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ContentType")] DataFile dataFile, HttpPostedFileBase file)
        {
            if (file == null) return View(dataFile);
            if (file.ContentLength == 0) return View(dataFile);

            using (BinaryReader reader = new BinaryReader(file.InputStream))
            {
                byte[] buff = reader.ReadBytes(file.ContentLength);

                //if (ModelState.IsValid)
                {
                    dataFile = new DataFile
                    {
                        ContentType = file.ContentType,
                        Name = file.FileName.Split('\\').Last(),
                        Content = buff,
                    };


                    db.DataFiles.Add(dataFile);
                    db.SaveChanges();

                }
            }
            return RedirectToAction("Index");
        }

        // GET: DataFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataFile dataFile = db.DataFiles.Find(id);
            if (dataFile == null)
            {
                return HttpNotFound();
            }
            return View(dataFile);
        }

        // POST: DataFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ContentType,Content")] DataFile dataFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataFile);
        }

        // GET: DataFiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataFile dataFile = db.DataFiles.Find(id);
            if (dataFile == null)
            {
                return HttpNotFound();
            }
            return View(dataFile);
        }

        // POST: DataFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataFile dataFile = db.DataFiles.Find(id);
            db.DataFiles.Remove(dataFile);
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
