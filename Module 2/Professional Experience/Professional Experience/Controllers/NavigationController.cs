using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Professional_Experience.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace Professional_Experience.Controllers
{
    public class NavigationController : Controller
    {
        //
        // GET: /Navigation/
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Menu()
        {
            if (User.IsInRole("Administrator"))
            {
                return PartialView("_NavigationAdministratorPartial");
            }
            if(User.IsInRole("Participant"))
            {
                return PartialView("_NavigationParticipantPartial");
            }
            return null;
        }

	}
}