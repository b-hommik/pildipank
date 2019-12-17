using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pildipank_ver2.Models;
using System.Threading.Tasks;


namespace Pildipank_ver2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            

            ParemEsimeneEntities db = new ParemEsimeneEntities();

            // Kõige uuemad pildid esilehele
          
                ViewBag.MostRecent = db.Pictures.OrderByDescending(x => x.Created).Where( x=> x.Visibility==true).Take(3).ToList();


                    // Kolm suvast pilti esilehele
            Random r = new Random();
            var pictures = db.Pictures.Where(x => x.Visibility == true).Select(x => x.Id).ToList();
            List<int> numbers = new List<int>();
            for (int i = 0; i<3; i++)
            {
                int n = r.Next(pictures.Count);
                numbers.Add(pictures[n]);
                pictures.RemoveAt(n);
            }
            //ViewBag.RandomPics=db.Pictures.Where(x => numbrid.Contains(x.Id)).ToList();
            ViewBag.RandomPics = numbers.Select(x => db.Pictures.Find(x)).ToList();


            // Kolm kõige rohkemate like'idega pilti esilehele 
            
            ViewBag.MostLiked= db.Pictures.Where(x => x.Visibility == true).OrderByDescending(x => x.Likes.Count()).Take(3).ToList();

            // Kõige populaarsemad albumid esilehele
            ViewBag.MostLikedAlbums = db.Albums.Where(x => x.Visibility == true).OrderByDescending(x => x.Pictures.Select(y=>y.Likes.Count()).Sum()/x.Pictures.Count()).Take(3).ToList();

            //db.Albums.OrderByDescending(a=>a.Sum(c=>c.Picture.Likes.Count())

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            if(Request.IsAuthenticated)
            {
                // teen midagi, mida kasutaja jaoks on vaja teha
            }
            else
            {
                // teen midagi mida võõra jaoks on vaja teha
            }
            return View();
        }
    }
}