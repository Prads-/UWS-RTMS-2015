using Professional_Experience.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Controllers
{
    [Authorize(Roles="Admin")]
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

        public ActionResult EditTrials()
        {
            var trials = _db.Trials.ToArray();

            return View(trials);
        }

        public ActionResult EditTrial(int id)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == id);

            if (trial != null)
            {
                var trialModel = new EditTrialViewModel();

                trialModel.Id = id;
                trialModel.Name = trial.Name;
                trialModel.Description = trial.Description;
                trialModel.StartDate = trial.Start_Date;
                trialModel.EndDate = trial.End_Date;
                trialModel.Objective = trial.Objectives;
                trialModel.Hypothesis = trial.Hypothesis;
                trialModel.Outcome = trial.Outcome;

                return View(trialModel);
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult EditTrial(EditTrialViewModel m)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == m.Id);

            if (trial == null)
            {
                ModelState.AddModelError("Edit", "Trial was not found!");
            }

            if (ModelState.IsValid)
            {
                trial.Name = m.Name;
                trial.Description = m.Description;
                trial.Start_Date = m.StartDate;
                trial.End_Date = m.EndDate;
                trial.Objectives = m.Objective;
                trial.Hypothesis = m.Hypothesis;
                trial.Outcome = m.Outcome;

                _db.SaveChanges();
                return View("Index");
            }

            return View(m);
        }

        public ActionResult AddParticipantToTrial()
        {
            var m = new AddParticipantToTrialViewModel();

            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.Participants = _db.Participants.OrderBy(p => p.Person.First_Name).ToArray();

            return View(m);
        }

        [HttpPost]
        public ActionResult AddParticipantToTrial(AddParticipantToTrialViewModel m)
        {
            var trialParticipants = _db.Trial_Participant.Where(tp => tp.Trial_Id == m.TrialId && tp.Participant_Id == m.ParticipantId);

            if (trialParticipants.Count() == 0)
            {
                PX_Model.Trial_Participant trialParticipant = new PX_Model.Trial_Participant();

                trialParticipant.Trial_Id = m.TrialId;
                trialParticipant.Participant_Id = m.ParticipantId;

                _db.Trial_Participant.Add(trialParticipant);
                _db.SaveChanges();

                return View("Index");
            }
            else
            {
                ModelState.AddModelError("Add", "Participant is already in this trial!");
            }

            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.Participants = _db.Participants.OrderBy(p => p.Person.First_Name).ToArray();

            return View(m);
        }

        public ActionResult ScreeningCriteria()
        {
            return View();
        }

        public ActionResult AddScreeningCriteria()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddScreeningCriteria(AddScreeningCriteriaViewModel m)
        {
            if (ModelState.IsValid)
            {
                var screeningCriteria = new PX_Model.Screening_Criteria();

                screeningCriteria.Description = m.Description;
                _db.Screening_Criteria.Add(screeningCriteria);
                _db.SaveChanges();

                foreach (var option in m.Options)
                {
                    var opt = new PX_Model.Screening_Criteria_Option();
                    opt.Screening_Criteria_Id = screeningCriteria.Id;
                    opt.Description = option;
                    _db.Screening_Criteria_Option.Add(opt);
                }
                _db.SaveChanges();

                return View("ScreeningCriteria");
            }
            return View();
        }

        public ActionResult AddScreeningCriteriaToTrial()
        {
            var m = new AddScreeningCriteriaToTrialViewModel();

            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.ScreeningCriteria = _db.Screening_Criteria.OrderBy(s => s.Description).ToArray();

            return View(m);
        }

        [HttpPost]
        public ActionResult AddScreeningCriteriaToTrial(AddScreeningCriteriaToTrialViewModel m)
        {
            m.OperatorValue = m.OperatorValue.Trim();

            if (m.OperatorType != PX_Model.Screening_Criteria.OPERATOR_EQUALS)
            {
                if (m.OperatorType == PX_Model.Screening_Criteria.OPERATOR_RANGE)
                {
                    string[] words = m.OperatorValue.Split(',');
                    if (words.Count() != 2)
                    {
                        ModelState.AddModelError("Add", "Range must have 2 numeric values separated by comma. Eg: 10,50");
                    }
                    else
                    {
                        decimal value1, value2;
                        words[0] = words[0].Trim();
                        words[1] = words[1].Trim();

                        bool r1 = decimal.TryParse(words[0], out value1);
                        bool r2 = decimal.TryParse(words[1], out value2);

                        if (!r1 || !r2)
                        {
                            ModelState.AddModelError("Add", "Both range values must be numeric");
                        }
                    }
                }
                else
                {
                    decimal value;
                    bool retVal = decimal.TryParse(m.OperatorValue, out value);
                    if (!retVal)
                    {
                        ModelState.AddModelError("Add", "Operator value must be numeric value");
                    }
                }

                var options = _db.Screening_Criteria_Option.Where(o => o.Screening_Criteria_Id == m.ScreeningCriteriaId);
                foreach (var option in options)
                {
                    decimal value;
                    bool retVal = decimal.TryParse(option.Description.Trim(), out value);
                    if (!retVal)
                    {
                        ModelState.AddModelError("Add", "One or more options in this selection criteria is non numeric. You can only choose 'Equals' as operator type.");
                        break;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var tsc = new PX_Model.Trial_Screening_Criteria();

                tsc.Trial_Id = m.TrialId;
                tsc.Screening_Criteria_Id = m.ScreeningCriteriaId;
                tsc.OperatorType = m.OperatorType;
                tsc.OperatorValue = m.OperatorValue;

                _db.Trial_Screening_Criteria.Add(tsc);
                _db.SaveChanges();

                return View("ScreeningCriteria");
            }

            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.ScreeningCriteria = _db.Screening_Criteria.OrderBy(s => s.Description).ToArray();

            return View(m);
        }
    }
}