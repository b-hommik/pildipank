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
    partial class AspNetUser
    {
        public string FullName => $"{FirstName} {LaststName}";
    }

}


namespace Pildipank_ver2.Controllers
{
    public class AspNetUsersController : Controller
    {
        private ParemEsimeneEntities db = new ParemEsimeneEntities();

        // GET: AspNetUsers
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
            {
                return View(db.AspNetUsers.ToList());
            }

        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }
        public ActionResult AddRole(string id, string roleId)
        {
            AspNetUser u = db.AspNetUsers.Find(id);
            AspNetRole r = db.AspNetRoles.Find(roleId);
            if (u != null && r != null)
            {
                u.AspNetRoles.Add(r);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = id });
        }


        // GET: AspNetUsers/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            //var age = GetAge(dt);
            //if (age < 18)
            //{
            //    ("Invalid Birth Day");
            //}

            //int GetAge(DateTime BirthDate)
            //{
            //    DateTime today = DateTime.Today;
            //    int age = today.Year - BirthDate.Year;
            //    if (BirthDate > today.AddYears(-age))
            //        age--;

            //    return age;
            //}
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirstName,LaststName,BirthDate,Hometown")] AspNetUser aspNetUser)
        {
            if ((aspNetUser.Email ?? "").Length < 5)
                ModelState.AddModelError("Email", "meiliaadress puudub või vigane");

            if(db.AspNetUsers.Where(x => x.Email == aspNetUser.Email).Count() > 0)
                ModelState.AddModelError("Email", "meiliaadress on juba registreeritud");

            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUser);
        }
        
            
         
        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirstName,LaststName,BirthDate,Hometown")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
