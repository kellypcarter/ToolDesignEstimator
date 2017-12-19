using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Boeing_Tool_Design.Models;

namespace Boeing_Tool_Design.Controllers
{
    public class HomeController : Controller
    {

        private BoeingContext db = new BoeingContext();

        [ChildActionOnly]
        public ActionResult NavBar()
        {
            int userlevel = 0;
            if (User.Identity.IsAuthenticated) {
                if (db.AppUsers.Where(d => d.UserEmail == User.Identity.Name).SingleOrDefault() != null)
                {
                    userlevel = db.AppUsers.Where(d => d.UserEmail == User.Identity.Name).SingleOrDefault().AccessLevelID;
                }
            }
            else {
                userlevel = 0;
            }
            return PartialView("_NavBar", userlevel);
        }

        [Authorize]
        public ActionResult Index()
        {
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

            return View();
        }

        public ActionResult StatConfig()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult EditEstimate()
        {
            ViewBag.Message = "estimates";
            return View();
        }
    }
}