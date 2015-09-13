using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Controllers
{
    [Authorize(Roles="Participant")]
    public class ParticipantController : Controller
    {
        // GET: Participant
        public ActionResult Index()
        {
            return View();
        }
    }
}