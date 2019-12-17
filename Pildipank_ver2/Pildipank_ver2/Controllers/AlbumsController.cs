using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pildipank_ver2.Models;

namespace Pildipank_ver2.Models
{
    partial class Album
    {
        ParemEsimeneEntities db = new ParemEsimeneEntities();
        public int OrderNo
        {
            get => OrderId == 0 ? Id : OrderId;
            set => OrderId = value;
        }

        public Picture CoverPicture => db.Pictures.Find(CoverPictureId) ?? db.Pictures.Find(25);

    }
}

namespace Pildipank_ver2.Controllers
{
    public class AlbumsController : Controller
    {
        private ParemEsimeneEntities db = new ParemEsimeneEntities();



        // GET: Albums
        public ActionResult Index(string sortKey = "Name", string sortDirection = "Asc", string searchString = "", int AlbumId = 0)
        {
            if (Request.IsAuthenticated)
            {
                var albums = db.Albums.Include(p => p.AspNetUser);

                var q = db.Albums
                    .Where(x => searchString == "" || x.Name.ToLower().Contains(searchString.ToLower()) || x.Description.ToLower().Contains(searchString.ToLower()))
                    .Where(x => AlbumId == 0 || x.Id == AlbumId)
                      ;
                var qs = q.OrderBy(x => x.Name);

                ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.Visibility == true), "Id", "Name", AlbumId);


                ViewBag.SortKey = sortKey;
                ViewBag.SortDirection = sortDirection;
                ViewBag.SearchString = searchString;


                switch (sortKey + sortDirection)
                {
                    case "NameAsc": qs = q.OrderBy(x => x.Name); break;
                    case "NameDesc": qs = q.OrderByDescending(x => x.Name); break;
                    case "FirstNameAsc": qs = q.OrderBy(x => x.AspNetUser.FirstName); break;
                    case "FirstNameDesc": qs = q.OrderByDescending(x => x.AspNetUser.FirstName); break;
                    case "CreatedAsc": qs = q.OrderBy(a => a.Pictures.Min(c => c.Created)); break;
                    case "CreatedDesc": qs = q.OrderByDescending(a => a.Pictures.Min(c => c.Created)); break;
                    case "MostLikedAsc": qs = q.OrderBy(x => x.Pictures.Select(y => y.Likes.Count()).Sum() / x.Pictures.Count()); break;
                    case "MostLikedDesc": qs = q.OrderByDescending(x => x.Pictures.Select(y => y.Likes.Count()).Sum() / x.Pictures.Count()); break;
                }

                return View(qs.ToList());
            }
            return RedirectToAction("Login", "Account");
        }


        //public ActionResult MoveUp(int id)
        //{
        //    Album a1 = db.Albums.Find(id);
        //    if (a1 != null)
        //    {
        //        Album a2 = db.Albums.ToList().Where(x => x.OrderNo < a1.OrderNo).OrderBy(x => x.OrderNo).LastOrDefault();
        //        if (a2 != null)
        //        {
        //            int uus = a2.OrderNo;
        //            a2.OrderNo = a1.OrderNo;
        //            a1.OrderNo = uus;
        //            db.SaveChanges();
        //        }

        //    }

        //    return RedirectToAction("Index");
        //}



        public ActionResult IndexUser(string sortKey = "Name", string sortDirection = "Asc", string searchString = "", int CategoryId = 0, int AlbumId = 0)
        {
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();
            var userAlbums = user.Albums;
            var albums = db.Albums.Include(p => p.AspNetUser).Include(p => p.CoverPictureId).Include(p => p.Description).Include(p => p.Name);

            var q = db.Albums
            .Where(x => searchString == "" || x.Name.ToLower().Contains(searchString.ToLower()) || x.Description.ToLower().Contains(searchString.ToLower()))
            .Where(x => AlbumId == 0 || x.Id == AlbumId).Where(x => x.AspNetUser.Id == user.Id)
              ;
            var qs = q.OrderBy(x => x.Name);

            ViewBag.AlbumId = new SelectList(db.Albums.Where(x => x.AspNetUser.Id == user.Id), "Id", "Name", AlbumId);


            ViewBag.SortKey = sortKey;
            ViewBag.SortDirection = sortDirection;
            ViewBag.SearchString = searchString;


            switch (sortKey + sortDirection)
            {
                case "NameAsc": qs = q.OrderBy(x => x.Name); break;
                case "NameDesc": qs = q.OrderByDescending(x => x.Name); break;
                case "CreatedAsc": qs = q.OrderBy(a => a.Pictures.Min(c => c.Created)); break;
                case "CreatedDesc": qs = q.OrderByDescending(a => a.Pictures.Min(c => c.Created)); break;
                case "MostLikedAsc": qs = q.OrderBy(x => x.Pictures.Select(y => y.Likes.Count()).Sum() / x.Pictures.Count()); break;
                case "MostLikedDesc": qs = q.OrderByDescending(x => x.Pictures.Select(y => y.Likes.Count()).Sum() / x.Pictures.Count()); break;
            }


            return View(qs.ToList());

            //return View(userAlbums.ToList());
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.PicturesInAlbum = db.Pictures.Where(x => x.AlbumId == id);
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).SingleOrDefault();
         

            ViewBag.CurrentUser = user;

            if (Request.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Album album = db.Albums.Find(id);
                if (album == null)
                {
                    return HttpNotFound();
                }
                return View(album);
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            if (!Request.IsAuthenticated) return RedirectToAction("Index");
            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();//kui Single ei kasuta, siis on kollektsioon.

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", user.Id);
            return View(new Album { UserId = user.Id });
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderId,UserId,Name,Description,Visibility,CoverPictureId=")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Add(album);
                db.SaveChanges();
                //return RedirectToAction("IndexUser");
                return RedirectToAction("Create", "Pictures");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", album.UserId);
            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", album.UserId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderId,UserId,Name,Description,Visibility,CoverPictureId")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexUser");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", album.UserId);

            AspNetUser user = db.AspNetUsers.Where(x => x.Email == User.Identity.Name).Single();
            return View(new Album { OrderId = 0, UserId = user.Id });
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);

            // Suuna pilte kustutama, siis saad tyhja albumi kustutada
            // Ei tee siia midagi

            db.SaveChanges();

            db.Albums.Remove(album);
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
