using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Controllers
{
    [Authorize(Roles="Participant")]
    public class ParticipantController : UIController
    {
        public PX_Model.Participant GetCurrentParticipant
        {
            get
            {
                string username = User.Identity.Name;
                var person = _db.People.FirstOrDefault(p => p.Username == username);
                return person.Participants.ElementAt(0);  
            }
        }

        // GET: Participant
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ParticipateTrialList()
        {
            var participant = GetCurrentParticipant;
            var trials = _db.Trials.ToArray();
            var trialList = new List<Professional_Experience.Models.ParticipateTrialListViewModel>();
            var trialParticipants = _db.Trial_Participant.Where(tp => tp.Participant_Id == participant.Id);

            foreach (var trial in trials)
            {
                var trialParticipant = trialParticipants.FirstOrDefault(tp => tp.Trial_Id == trial.Id);
                if (trialParticipant == null)
                {
                    var m = new Professional_Experience.Models.ParticipateTrialListViewModel();
                    m.TrialId = trial.Id;
                    m.TrialName = trial.Name;
                    m.TrialDescription = trial.Description;
                    trialList.Add(m);
                }
            }

            return View(trialList.AsEnumerable());
        }

        public ActionResult ParticipateTrial(int id)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == id);
            var trialScreeningCriteria = _db.Trial_Screening_Criteria.Where(sc => sc.Trial_Id == id);

            if (trialScreeningCriteria.Count() == 0)
            {
                createTrialParticipant(id);
                return View("ScreeningCriteriaSuccess");
            }

            var m = new List<Professional_Experience.Models.ScreeningCriteriaViewModel>();

            foreach (var tsc in trialScreeningCriteria)
            {
                var model = new Professional_Experience.Models.ScreeningCriteriaViewModel();
                model.TrialScreeningCriteriaId = tsc.Id;
                model.Description = tsc.Screening_Criteria.Description;
                if (tsc.Screening_Criteria.Screening_Criteria_Option.Count > 0)
                {
                    model.Options = new List<string>();
                    foreach (var option in tsc.Screening_Criteria.Screening_Criteria_Option)
                    {
                        model.Options.Add(option.Description);
                    }
                }
                m.Add(model);
            }
            ViewBag.tid = id;

            return View(m.AsEnumerable());
        }

        [HttpPost]
        public ActionResult ParticipateTrial(int tid, IEnumerable<Professional_Experience.Models.ScreeningCriteriaViewModel> m)
        {
            foreach (var model in m) {
                var tsc = _db.Trial_Screening_Criteria.FirstOrDefault(t => t.Id == model.TrialScreeningCriteriaId);
                if (tsc.OperatorType == PX_Model.Screening_Criteria.OPERATOR_EQUALS)
                {
                    if (model.Answer != tsc.OperatorValue)
                    {
                        return View("ScreeningCriteriaFailed");
                    }
                }
                else
                {
                    decimal answer, operatorValue = 0;
                    bool r = decimal.TryParse(model.Answer, out answer);
                    if (!r)
                    {
                        ModelState.AddModelError("", "Invalid answer! Answer must be numeric!");
                        return RedirectToAction("ParticipateTrial", new { id = tid });
                    }
                    if (tsc.OperatorType != PX_Model.Screening_Criteria.OPERATOR_RANGE)
                    {
                        operatorValue = decimal.Parse(tsc.OperatorValue);
                    }
                    bool pass = false;
                    switch (tsc.OperatorType)
                    {
                        case PX_Model.Screening_Criteria.OPERATOR_GREATER_THAN:
                            pass = answer > operatorValue;
                            break;
                        case PX_Model.Screening_Criteria.OPERATOR_GREATER_THAN_OR_EQUAL:
                            pass = answer >= operatorValue;
                            break;
                        case PX_Model.Screening_Criteria.OPERATOR_LESS_THAN:
                            pass = answer < operatorValue;
                            break;
                        case PX_Model.Screening_Criteria.OPERATOR_LESS_THAN_OR_EQUAL:
                            pass = answer <= operatorValue;
                            break;
                        case PX_Model.Screening_Criteria.OPERATOR_RANGE:
                            decimal upper, lower;
                            string[] words = tsc.OperatorValue.Split(',');
                            words[0] = words[0].Trim();
                            words[1] = words[1].Trim();
                            
                            lower = decimal.Parse(words[0]);
                            upper = decimal.Parse(words[1]);

                            pass = answer >= lower && answer <= upper;
                            break;
                    }
                    if (!pass)
                    {
                        return View("ScreeningCriteriaFailed");
                    }
                }
            }
            createTrialParticipant(tid);

            return View("ScreeningCriteriaSuccess");
        }

        private void createTrialParticipant(int tid)
        {
            int pid = GetCurrentParticipant.Id;
            var tp = new PX_Model.Trial_Participant();
            tp.Trial_Id = tid;
            tp.Participant_Id = pid;
            _db.Trial_Participant.Add(tp);
            _db.SaveChanges();
        }

        public ActionResult ScreeningCriteriaFailed()
        {
            return View();
        }

        public ActionResult ScreeningCriteriaSuccess()
        {
            return View();
        }
    }
}