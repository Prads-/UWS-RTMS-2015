using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Controllers
{
   [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        // GET: Administrator
        public ActionResult Index()
        {
            return View();
        }

       public ActionResult InterventionSetup()
        {
            return View();
        }
        public ActionResult Reporting()
        {
            return View();
        }
    }
}