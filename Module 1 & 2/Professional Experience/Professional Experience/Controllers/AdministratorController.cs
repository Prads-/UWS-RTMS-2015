using Professional_Experience.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Helpers;
using PagedList;

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

        public ActionResult Trial()
        {
            return View();
        }

        public ActionResult Investigator()
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
                trial.TermsAndConditions = m.TermsAndConditions;
                trial.HasBeenRandomised = false;

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
                trialModel.TermsAndConditions = trial.TermsAndConditions;

                return View(trialModel);
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult EditTrial(EditTrialViewModel m)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == m.Id);

            if (trial != null)
            {
                trial.Name = m.Name;
                trial.Description = m.Description;
                trial.Start_Date = m.StartDate;
                trial.End_Date = m.EndDate;
                trial.Objectives = m.Objective;
                trial.Hypothesis = m.Hypothesis;
                trial.Outcome = m.Outcome;
                trial.TermsAndConditions = m.TermsAndConditions;

                _db.SaveChanges();
            }

            return View("Index");
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

                if (m.Options != null)
                {
                    foreach (var option in m.Options)
                    {
                        var opt = new PX_Model.Screening_Criteria_Option();
                        opt.Screening_Criteria_Id = screeningCriteria.Id;
                        opt.Description = option;
                        _db.Screening_Criteria_Option.Add(opt);
                    }
                    _db.SaveChanges();
                }

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

        public ActionResult CreateInvestigator()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateInvestigator(RegisterInvestigatorViewModel m)
        {
            if (ModelState.IsValid)
            {
                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = new ApplicationUser { UserName = m.Username, Email = m.Email };
                var result = await UserManager.CreateAsync(user, m.Password);
                if (result.Succeeded)
                {
                    var person = new PX_Model.Person();

                    person.First_Name = m.FirstName;
                    person.Last_Name = m.LastName;
                    person.Email = m.Email;
                    person.Phone_Number = m.PhoneNumber;
                    person.Postcode = m.Postcode;
                    person.State = m.State;
                    person.Street = m.Street;
                    person.Suburb = m.Suburb;
                    person.Username = m.Username;

                    _db.People.Add(person);
                    _db.SaveChanges();

                    var investigator = new PX_Model.Investigator();

                    investigator.Person_Id = person.Id;
                    investigator.Institution = m.Institution;

                    _db.Investigators.Add(investigator);
                    _db.SaveChanges();

                    return View("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(m);
        }

        public ActionResult EditInvestigators()
        {
            IEnumerable<PX_Model.Investigator> m = _db.Investigators.ToArray();
            return View(m);
        }

        public ActionResult EditInvestigator(int id)
        {
            var investigator = _db.Investigators.FirstOrDefault(i => i.Id == id);

            if (investigator == null)
            {
                return View("Index");
            }

            var m = new EditInvestigatorViewModel();
            
            m.Id = id;
            m.Email = investigator.Person.Email;
            m.FirstName = investigator.Person.First_Name;
            m.Institution = investigator.Institution;
            m.LastName = investigator.Person.Last_Name;
            m.PhoneNumber = investigator.Person.Phone_Number;
            m.Postcode = investigator.Person.Postcode;
            m.State = investigator.Person.State;
            m.Street = investigator.Person.Street;
            m.Suburb = investigator.Person.Suburb;
            
            return View(m);
        }

        [HttpPost]
        public ActionResult EditInvestigator(EditInvestigatorViewModel m)
        {
            var investigator = _db.Investigators.FirstOrDefault(i => i.Id == m.Id);

            if (investigator != null)
            {
                investigator.Institution = m.Institution;
                investigator.Person.Email = m.Email;
                investigator.Person.First_Name = m.FirstName;
                investigator.Person.Last_Name = m.LastName;
                investigator.Person.Phone_Number = m.PhoneNumber;
                investigator.Person.Postcode = m.Postcode;
                investigator.Person.State = m.State;
                investigator.Person.Street = m.Street;
                investigator.Person.Suburb = m.Suburb;

                _db.SaveChanges();
            }

            return View("Index");
        }

        public ActionResult AssignInvestigatorToTrial()
        {
            var m = new AssignInvestigatorToTrialViewModel();

            m.Investigators = _db.Investigators.OrderBy(i => i.Person.First_Name).ToArray();
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();

            return View(m);
        }

        [HttpPost]
        public ActionResult AssignInvestigatorToTrial(AssignInvestigatorToTrialViewModel m)
        {
            var trialInvestigator = new PX_Model.Trial_Investigator();

            trialInvestigator.Investigator_Id = m.InvestigatorId;
            trialInvestigator.Trial_Id = m.TrialId;
            trialInvestigator.Type = m.Type;

            _db.Trial_Investigator.Add(trialInvestigator);
            _db.SaveChanges();

            return View("Index");
        }

        public ActionResult CreateBaselineAssessment()
        {
            var m = new CreateBaselineAssessmentViewModel();
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            return View(m);
        }

        [HttpPost]
        public ActionResult CreateBaselineAssessment(CreateBaselineAssessmentViewModel m)
        {
            if (ModelState.IsValid)
            {
                var assessmentType = new PX_Model.Assessment_Type();
                assessmentType.Name = m.Name;
                assessmentType.Trial_Id = m.TrialId;
                assessmentType.Description = m.Description;
                _db.Assessment_Type.Add(assessmentType);
                _db.SaveChanges();

                foreach (var question in m.Questions)
                {
                    if (question.AddToQuestion)
                    {
                        var assessmentTypeQuestion = new PX_Model.Assessment_Type_Question();
                        assessmentTypeQuestion.Question = question.Question;
                        assessmentTypeQuestion.Question_Type = question.Type;
                        assessmentTypeQuestion.Assessment_Type_Id = assessmentType.Id;
                        _db.Assessment_Type_Question.Add(assessmentTypeQuestion);
                        _db.SaveChanges();

                        if (question.Type != PX_Model.Assessment_Type_Question.TYPE_TEXT)
                        {
                            foreach (var option in question.Options)
                            {
                                var opt = new PX_Model.Assessment_Type_Option();
                                opt.Opt = option;
                                opt.Assessment_Type_Question_Id = assessmentTypeQuestion.Id;
                                _db.Assessment_Type_Option.Add(opt);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
                return View("Index");
            }
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            return View(m);
        }

        private List<PX_Model.Trial> getNonRandomisedTrials()
        {
            var trials = _db.Trials;
            var trialList = new List<PX_Model.Trial>();

            foreach (var t in trials)
            {
                if (t.HasBeenRandomised == false)
                {
                    trialList.Add(t);
                }
            }
            return trialList;
        }

        public ActionResult RandomiseTrialParticipants()
        {
            var trialList = getNonRandomisedTrials();

            if (trialList.Count == 0)
            {
                ModelState.AddModelError("", "There are no trials that needs to be randomised");
                return View("Index");
            }

            var m = new RandomiseViewModel();
            m.Trials = trialList.AsEnumerable();

            return View(m);
        }

        [HttpPost]
        public ActionResult RandomiseTrialParticipants(Professional_Experience.Models.RandomiseViewModel m)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == m.TrialId);

            if (trial == null)
            {
                ModelState.AddModelError("", "Trial does not exist");
            }

            if (m.NumberOfParticipantInIntervention < 0 || trial.Trial_Participant.Count < m.NumberOfParticipantInIntervention)
            {
                ModelState.AddModelError("", "Invalid number of participants to be added in intervention group");
            }

            if (ModelState.IsValid)
            {
                var trialParticipants = trial.Trial_Participant;

                Random rnd = new Random();
                int count = trialParticipants.Count;
                int random = rnd.Next(0, count);
                
                for (int i = 0; i < m.NumberOfParticipantInIntervention; ++i)
                {
                    trialParticipants.ElementAt((i + random)  % count).Classification = PX_Model.Trial_Participant.CLASSIFICATION_INTERVENTION;
                }
                for (int i = m.NumberOfParticipantInIntervention; i < count; ++i)
                {
                    trialParticipants.ElementAt((i + random) % count).Classification = PX_Model.Trial_Participant.CLASSIFICATION_CONTROL;
                }
                trial.HasBeenRandomised = true;
                _db.SaveChanges();

                return View("Index");
            }

            m.Trials = getNonRandomisedTrials().AsEnumerable();
            return View(m);
        }

        public ActionResult ViewTrials()
        {
            var m = new Professional_Experience.Models.AdminViewTrialsViewModel();
            m.Randomised = _db.Trials.Where(t => t.HasBeenRandomised == true);
            m.NonRandomised = _db.Trials.Where(t => t.HasBeenRandomised == false);
            return View(m);
        }

        public ActionResult ViewRandomisedTrial(int id)
        {
            var m = new ViewRandomisedViewModel();
            m.Trial = _db.Trials.FirstOrDefault(t => t.Id == id);
            if (m.Trial == null)
            {
                ModelState.AddModelError("", "Trial not found!");
                return View("Index");
            }
            m.ParticipantGroups = _db.Participant_Group.Where(pg => pg.Trial_Participant_Participant_Group.FirstOrDefault(tppg => tppg.Trial_Participant.Trial_Id == id) != null);
            
            return View(m);
        }

        public ActionResult ViewNonRandomisedTrial(int id)
        {
            var m = _db.Trials.FirstOrDefault(t => t.Id == id);
            if (m == null)
            {
                ModelState.AddModelError("", "Trial not found!");
                return View("Index");
            }
            return View(m);
        }

        public ActionResult ViewAllParticipants(int id, int page = 0)
        {
            var trialParticipants = _db.Trial_Participant.
                Where(tp => tp.Trial_Id == id).OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            var m = new PagedList<PX_Model.Trial_Participant>(trialParticipants, page, 15);
            return View(m);
        }

        public ActionResult ViewAllInterventionParticipant(int id)
        {
            var m = _db.Trial_Participant.
                Where(tp => tp.Trial_Id == id && tp.Classification == PX_Model.Trial_Participant.CLASSIFICATION_INTERVENTION).
                OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            return View("ViewAllParticipants", m);
        }

        public ActionResult ViewAllExperimentalParticipant(int id)
        {
            var m = _db.Trial_Participant.
                Where(tp => tp.Trial_Id == id && tp.Classification == PX_Model.Trial_Participant.CLASSIFICATION_CONTROL).
                OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            return View("ViewAllParticipants", m);
        }

        public ActionResult ViewParticipantsInGroup(int id, int pgid)
        {
            var m = _db.Trial_Participant.Where(tp => tp.Id == id && tp.Trial_Participant_Participant_Group.Where(tppg => tppg.Participant_Group.Id == pgid).Count() > 0);
            return View("ViewAllParticipants", m);
        }

        public ActionResult ViewParticipant(int id)
        {
            var m = _db.Participants.FirstOrDefault(p => p.Id == id);
            if (m == null)
            {
                ModelState.AddModelError("", "Participant not found");
                return View("Index");
            }
            return View(m);
        }

        public ActionResult Report()
        {
            return View(_db.Trials.OrderBy(t => t.Name).AsEnumerable());
        }

        public ActionResult GeneralReport()
        {
            var trials = _db.Trials;
            var trialNames = new List<string>();
            var numberOfParticipants = new List<int>();

            foreach (var t in trials)
            {
                trialNames.Add(t.Name);
                numberOfParticipants.Add(t.Trial_Participant.Count);
            }

            var barChart = new Chart(width: 600, height: 400)
                .AddTitle("Number of participant on trials")
                .AddSeries(
                    name: "Trials",
                    xValue: trialNames.ToArray(),
                    yValues: numberOfParticipants.ToArray());
            barChart.Save("~/Charts/bar.jpg");

            return View(trials);
        }

        public ActionResult TrialReport(int id)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == id);
            if (trial == null)
            {
                ModelState.AddModelError("", "Trial not found");
                return View("Index");
            }
            var m = new Professional_Experience.Models.TrialReportViewModel();
            m.TrialName = trial.Name;
            m.NumberOfMales = trial.Trial_Participant.Where(tp => tp.Participant.Gender == "Male").Count();
            m.NumberOfFemales = trial.Trial_Participant.Count - m.NumberOfMales;
            m.NumberOfInterventionParticipant = trial.Trial_Participant.Where(tp => tp.Classification == PX_Model.Trial_Participant.CLASSIFICATION_INTERVENTION).Count();
            m.NumberOfExperimentalParticipant = trial.Trial_Participant.Count - m.NumberOfInterventionParticipant;

            var genderChart = new Chart(width: 600, height: 400)
                .AddTitle("Number of participants according to gender")
                .AddSeries(
                    chartType: "Pie",
                    name: "Trials",
                    xValue: new[] { "Male", "Female" },
                    yValues: new[] { m.NumberOfMales, m.NumberOfFemales });
            genderChart.Save("~/Charts/gender.jpg");

            var classificationChart = new Chart(width: 600, height: 400)
                .AddTitle("Number of participants according to classification")
                .AddSeries(
                    chartType: "Pie",
                    name: "Trials",
                    xValue: new[] { "Intervention", "Experimental" },
                    yValues: new[] { m.NumberOfInterventionParticipant, m.NumberOfExperimentalParticipant });
            classificationChart.Save("~/Charts/classification.jpg");

            return View(m);
        }
    }
}