using Professional_Experience.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Controllers
{
    public class AdministratorController : UIController
    {
        // GET: Administrator
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateTrial()
        {
            var m = new CreateTrialViewModel();
            m.StartDate = DateTime.Now;
            m.EndDate = DateTime.Now;
            return View(m);
        }

        [HttpPost]
        public ActionResult CreateTrial(CreateTrialViewModel m)
        {
            if (ModelState.IsValid)
            {
                var trial = new PX_Model.Trial();

                trial.Name = m.Name;
                trial.Description = m.Description;
                trial.Start_Date = m.StartDate;
                trial.End_Date = m.EndDate;
                trial.Hypothesis = m.Hypothesis;
                trial.Outcome = m.Outcome;
                trial.Objectives = m.Objective;

                _db.Trials.Add(trial);
                _db.SaveChanges();

                return View("Index");
            }
            return View(m);
        }
    }
}