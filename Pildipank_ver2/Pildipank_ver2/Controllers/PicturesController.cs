using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pildipank_ver2.Models;
using System.IO;

namespace Pildipank_ver2.Controllers
{
    public class PicturesController : Controller
    {
        private ParemEsimeneEntities db = new ParemEsimeneEntities();


        // GET: Pictures
        public ActionResult Index(string sortKey = "Created", string sortDirection = "Asc", string searchString = "", int CategoryId = 0)

        {
            if (Request.IsAuthenticated)
            {
                var pictures = db.Pictures.Include(p => p.Album).Include(p => p.AspNetUser).Include(p => p.Category);

                var q = db.Pictures
                  .Where(x => searchString == "" || x.Name.ToLower().Contains(searchString.ToLower()) || x.Description.ToLower().Contains(searchString.ToLower()))
                  .Where(x => CategoryId == 0 || x.CategoryId == CategoryId)
                  ;
                var qs = q.OrderBy(x => x.Created);

                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", CategoryId);


                ViewBag.SortKey = sortKey;
                ViewBag.SortDirection = sortDirection;
                ViewBag.SearchString = searchString;

                ViewBag.AllPictures = db.Pictures.OrderByDescending(x => x.Created).Where(x => x.Visibility == true).ToList();


                switch (sortKey + sortDirection)
                {
                    case "CreatedAsc": qs = q.OrderBy(x => x.Created); break;
                    case "CreatedDesc": qs = q.OrderByDescending(x => x.Created); break;
                    case "MostLikedAsc": qs = q.OrderBy(x => x.Likes.Count()); break;
                    case "MostLikedDesc": qs = q.OrderByDescending(x => x.Likes.Count()); break;
                }




                return View(qs.ToList());
            }
            return RedirectToAction("Login", "Account");
        }
        // meetod, mis seab coverpicture
        public ActionResult SetCoverPicture(int id) //, IEnumerable<bool> Chbxs)
        {
            Picture picture = db.Pictures.Find(id);
            
            {
                if (picture != null)
                {
                    picture.Album.CoverPictureId = picture.Id;
                    db.SaveChanges();
                }
            }


            return RedirectToAction("IndexUser");
        }
        // Most recent pictures



        // Siin Laikimise meetodid
        public ActionResult AddLike(int id)
        {
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).SingleOrDefault();
            Picture picture = db.Pictures.Find(id);

            if (user != null && picture != null)
            {
                if (picture.Likes.Where(x => x.LikerId == user.Id).Count() == 0)
                {
                    try
                    {
                        picture.Likes.Add(new Like
                        {
                            LikerId = user.Id,
                            Number = 1
                        });
                        db.SaveChanges();
                    }
                    catch { }
                }


            }
            return RedirectToAction("Details", "Pictures",new { id = picture.Id });
        }

        // Kommentaaride lismaine
        public ActionResult AddComment(int id, string comment)
        {
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).SingleOrDefault();
            Picture picture = db.Pictures.Find(id);


            if (user != null && picture != null)
            {


                try
                {
                    picture.Comments.Add(new Comment
                    {
                        CommenterId = user.Id,
                        Created = DateTime.Now,
                        Comment1 = comment
                    });
                    db.SaveChanges();
                }
                catch { }



            }
            return RedirectToAction("Details", new { id = id });
        }

        // Kasutaja näeb kõiki enda pilte
        public ActionResult IndexUser(string sortKey = "Created", string sortDirection = "Asc", string searchString = "", int CategoryId = 0, int AlbumId=0)
        {
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();
            var userPictures = user.Pictures;


            var q = userPictures
                .Where(x => searchString == "" || x.Name.ToLower().Contains(searchString.ToLower()) || x.Description.ToLower().Contains(searchString.ToLower()))
              .Where(x => CategoryId == 0 || x.CategoryId == CategoryId)
              .Where(x => AlbumId == 0 || x.AlbumId == AlbumId).Where(x => x.AspNetUser.Id == user.Id)
              ;
            var qs = q.OrderBy(x => x.Created);

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", CategoryId);
            ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.AspNetUser.Id == user.Id), "Id", "Name", AlbumId);

            ViewBag.SortKey = sortKey;
            ViewBag.SortDirection = sortDirection;
            ViewBag.SearchString = searchString;


            switch (sortKey + sortDirection)
            {
                case "CreatedAsc": qs = q.OrderBy(x => x.Created); break;
                case "CreatedDesc": qs = q.OrderByDescending(x => x.Created); break;
                case "MostLikedAsc": qs = q.OrderBy(x => x.Likes.Count()); break;
                case "MostLikedDesc": qs = q.OrderByDescending(x => x.Likes.Count()); break;
            }
            
            return View(qs.ToList());
        }

       

        // GET: Pictures/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            
            AspNetUser user1 = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).SingleOrDefault();


            ViewBag.CurrentUser = user1;

            if (!Request.IsAuthenticated) return RedirectToAction("Login", "Account");
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();

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

        // GET: Pictures/Create
        [Authorize]
        public ActionResult Create()
        {
            if (!Request.IsAuthenticated) return RedirectToAction("Index");
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FirstName", user.Id);
            ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.AspNetUser.Id == user.Id), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            


            return View(new Picture { UserId = user.Id, Created = DateTime.Now });
        }


        // POST: Pictures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,Description,Created,Visibility,AlbumId,CategoryId")] Picture picture, HttpPostedFileBase file)
        {
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                using (BinaryReader reader = new BinaryReader(file.InputStream))
                {
                    byte[] buff = reader.ReadBytes(file.ContentLength);


                    {
                        DataFile dataFile = new DataFile
                        {
                            ContentType = file.ContentType,
                            Name = file.FileName.Split('\\').Last(),
                            Content = buff,
                        };


                        db.DataFiles.Add(dataFile);
                        db.SaveChanges();

                        picture.FileId = dataFile.Id;
                        db.Pictures.Add(picture);
                        db.SaveChanges();
                        return RedirectToAction("IndexUser");
                    }


                }


            }

            ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.AspNetUser.Id == user.Id), "Id", "UserId", picture.AlbumId);

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", picture.CategoryId);
            return View(picture);
        }

        // GET: Pictures/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            //if (!Request.IsAuthenticated) return RedirectToAction("Login", "Account");
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Picture picture = db.Pictures.Find(id);
            if (picture == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.AspNetUser.Id == user.Id), "Id", "Name", picture.AlbumId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FirstName", picture.UserId);
            ViewBag.FileId = new SelectList(db.DataFiles, "Id", "Name", picture.FileId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", picture.CategoryId);
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,UserId,FileId,Created,Name,Description,Visibility,AlbumId,CategoryId")] Picture picture)
        {
            //if (!Request.IsAuthenticated) return RedirectToAction("Login");
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();

            if (ModelState.IsValid)
            {
                db.Entry(picture).State = EntityState.Modified;
                //try
                //{
                db.SaveChanges();
                //}
                //catch { };
                return RedirectToAction("IndexUser");
            }
            ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.AspNetUser.Id == user.Id), "Id", "Name", picture.AlbumId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", picture.UserId);
            ViewBag.FileId = new SelectList(db.DataFiles, "Id", "Name", picture.FileId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", picture.CategoryId);
            return View(picture);
        }

        // GET: Pictures/Delete/5
        [Authorize]
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

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Picture picture = db.Pictures.Find(id);

            // Siia vahele teha tehted, et kõik, mis Pildi küljes on (kommentaar, like) ka remove-ida ja siis picture ka.

            var likes = picture.Likes.ToList();
            var comments = picture.Comments.ToList();
            foreach (var l in likes) db.Likes.Remove(l);
            foreach (var c in comments) db.Comments.Remove(c);

            db.SaveChanges();

            db.Pictures.Remove(picture);
            db.SaveChanges();
            return RedirectToAction("IndexUser");
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
