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
    //This is the admin controller and all the administration logic is inside this controller
    [Authorize(Roles="Admin")]
    public class AdministratorController : UIController
    {
        //Show Dashboard
        public ActionResult Index()
        {
            return View();
        }

        //Show Trial section
        public ActionResult Trial()
        {
            return View();
        }

        //Show Investigator section
        public ActionResult Investigator()
        {
            return View();
        }

        //Show create trial form
        public ActionResult CreateTrial()
        {
            var m = new CreateTrialViewModel();
            m.StartDate = DateTime.Now;
            m.EndDate = DateTime.Now;
            return View(m);
        }

        //Get the input from create trial form
        [HttpPost]
        public ActionResult CreateTrial(CreateTrialViewModel m)
        {
            //If input is valid
            if (ModelState.IsValid)
            {
                //Create a trial
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

                //Push the trial to the database
                _db.Trials.Add(trial);
                _db.SaveChanges();

                //Go to the dashboard
                return View("Index");
            }
            //If there was an error, redisplay the form with the errors
            return View(m);
        }

        //This method searches for a trial using search word
        public ActionResult SearchTrials(string searchWord)
        {
            searchWord = searchWord.ToLower();
            //This looksup the trial with search word in Name and Description fields
            var trials = _db.Trials.Where(t => t.Name.ToLower().Contains(searchWord) || t.Description.ToLower().Contains(searchWord));
            return View("EditTrials", new PagedList<PX_Model.Trial>(trials.OrderBy(t => t.Name), 1, trials.Count() + 1));
        }

        //Show all the trials from database
        public ActionResult EditTrials(int page = 1)
        {
            //Get all the trials from the database
            var trials = new PagedList<PX_Model.Trial>(_db.Trials.AsEnumerable().OrderBy(t => t.Name), page, 7);
            return View(trials);
        }

        //Show form to edit trial
        public ActionResult EditTrial(int id)
        {
            //Get the trial we're editing
            var trial = _db.Trials.FirstOrDefault(t => t.Id == id);

            //If trial is in the database
            if (trial != null)
            {
                //Create a model to store the trial
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

                //Send the trial data to the view
                return View(trialModel);
            }

            //If there was an error, go to index and show the error
            return View("Index");
        }

        //Get user input and edit the trial
        [HttpPost]
        public ActionResult EditTrial(EditTrialViewModel m)
        {
            //Get the trial from database
            var trial = _db.Trials.FirstOrDefault(t => t.Id == m.Id);

            //If trial is in the database
            if (trial != null)
            {
                //Save the user input in the trial object
                trial.Name = m.Name;
                trial.Description = m.Description;
                trial.Start_Date = m.StartDate;
                trial.End_Date = m.EndDate;
                trial.Objectives = m.Objective;
                trial.Hypothesis = m.Hypothesis;
                trial.Outcome = m.Outcome;
                trial.TermsAndConditions = m.TermsAndConditions;

                //Save the changes we made in trial object in our dataabase
                _db.SaveChanges();
            }

            //Return to the admin dashboard page
            return View("Index");
        }

        //Show the form for adding participant to database
        public ActionResult AddParticipantToTrial()
        {
            var m = new AddParticipantToTrialViewModel();

            //Get all trials and participant from the database
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.Participants = _db.Participants.OrderBy(p => p.Person.First_Name).ToArray();

            //Pass them on to the view
            return View(m);
        }

        //Admin sends the participant they want to add to the database to this function
        [HttpPost]
        public ActionResult AddParticipantToTrial(AddParticipantToTrialViewModel m)
        {
            //Get the trial participant associated with this trial
            var trialParticipants = _db.Trial_Participant.Where(tp => tp.Trial_Id == m.TrialId && tp.Participant_Id == m.ParticipantId);
            
            //If there are not trial participant associated
            if (trialParticipants.Count() == 0)
            {
                //Create trial participant
                PX_Model.Trial_Participant trialParticipant = new PX_Model.Trial_Participant();

                trialParticipant.Trial_Id = m.TrialId;
                trialParticipant.Participant_Id = m.ParticipantId;

                //Add trial participant
                _db.Trial_Participant.Add(trialParticipant);
                _db.SaveChanges();

                //Redirect to admin dashboard
                return View("Index");
            }
            else
            {
                //If there is already a trial participant object
                ModelState.AddModelError("Add", "Participant is already in this trial!");
            }

            //If there was an error, get the trial and participant lists and show the form again
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.Participants = _db.Participants.OrderBy(p => p.Person.First_Name).ToArray();

            return View(m);
        }

        //Show screening criteria section
        public ActionResult ScreeningCriteria()
        {
            return View();
        }

        //Show create screening criteria form
        public ActionResult AddScreeningCriteria()
        {
            return View();
        }

        //Get the created screening criteria
        [HttpPost]
        public ActionResult AddScreeningCriteria(AddScreeningCriteriaViewModel m)
        {
            //If there was no problem with input data
            if (ModelState.IsValid)
            {
                //Create screening criteria object and add it to the database
                var screeningCriteria = new PX_Model.Screening_Criteria();

                screeningCriteria.Description = m.Description;
                _db.Screening_Criteria.Add(screeningCriteria);
                _db.SaveChanges();

                //If there are options
                if (m.Options != null)
                {
                    //Add options to the database
                    foreach (var option in m.Options)
                    {
                        var opt = new PX_Model.Screening_Criteria_Option();
                        opt.Screening_Criteria_Id = screeningCriteria.Id;
                        opt.Description = option;
                        _db.Screening_Criteria_Option.Add(opt);
                    }
                    _db.SaveChanges();
                }

                //Go to screening criteria section page
                return View("ScreeningCriteria");
            }
            //If error, show the form again
            return View();
        }

        //Show the form for assigning screening criteria to trial
        public ActionResult AddScreeningCriteriaToTrial()
        {
            var m = new AddScreeningCriteriaToTrialViewModel();

            //Get the list of trials and screening criteria
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.ScreeningCriteria = _db.Screening_Criteria.OrderBy(s => s.Description).ToArray();

            //Send them off to the view
            return View(m);
        }

        //Add the screening criteria to the trial
        [HttpPost]
        public ActionResult AddScreeningCriteriaToTrial(AddScreeningCriteriaToTrialViewModel m)
        {
            //Get rid of spaces at the front and back of operator value
            m.OperatorValue = m.OperatorValue.Trim();

            if (m.OperatorType != PX_Model.Screening_Criteria.OPERATOR_EQUALS)
            {
                if (m.OperatorType == PX_Model.Screening_Criteria.OPERATOR_RANGE)
                {
                    //Get the two values for the range
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

                        //Get the two values as decimal
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
                    //Get the numerical value of the input
                    decimal value;
                    bool retVal = decimal.TryParse(m.OperatorValue, out value);
                    if (!retVal)
                    {
                        ModelState.AddModelError("Add", "Operator value must be numeric value");
                    }
                }

                //Get all the options and make sure they are numeric as well
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

            //If everything's alright
            if (ModelState.IsValid)
            {
                //Add the screening criteria to the database
                var tsc = new PX_Model.Trial_Screening_Criteria();

                tsc.Trial_Id = m.TrialId;
                tsc.Screening_Criteria_Id = m.ScreeningCriteriaId;
                tsc.OperatorType = m.OperatorType;
                tsc.OperatorValue = m.OperatorValue;

                _db.Trial_Screening_Criteria.Add(tsc);
                _db.SaveChanges();

                return View("ScreeningCriteria");
            }

            //Else get the list of trial and screening criteria again and show the form with error messages
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            m.ScreeningCriteria = _db.Screening_Criteria.OrderBy(s => s.Description).ToArray();

            return View(m);
        }

        //Show create investigator form
        public ActionResult CreateInvestigator()
        {
            return View();
        }

        //Get input from the create investigator form
        [HttpPost]
        public async Task<ActionResult> CreateInvestigator(RegisterInvestigatorViewModel m)
        {
            //If inputs are valid
            if (ModelState.IsValid)
            {
                //Create a login account
                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = new ApplicationUser { UserName = m.Username, Email = m.Email };
                var result = await UserManager.CreateAsync(user, m.Password);
                
                //If we successfully created a login account
                if (result.Succeeded)
                {
                    //Create a person in the database
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

                    //Create an investigator in the database
                    var investigator = new PX_Model.Investigator();

                    investigator.Person_Id = person.Id;
                    investigator.Institution = m.Institution;

                    _db.Investigators.Add(investigator);
                    _db.SaveChanges();

                    //Go to admin dashboard
                    return View("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            //If error, show the form again with error messages on the top
            return View(m);
        }

        //Show list of investigator in the database
        public ActionResult EditInvestigators()
        {
            //Get the list of investigator and pass it on to the view
            IEnumerable<PX_Model.Investigator> m = _db.Investigators.ToArray();
            return View(m);
        }

        //Show a form to edit investigator
        public ActionResult EditInvestigator(int id)
        {
            //Get the id of the investigator we're trying to edit
            var investigator = _db.Investigators.FirstOrDefault(i => i.Id == id);

            //If there is no investigator with that id, just go back to the dashboard
            if (investigator == null)
            {
                return View("Index");
            }

            //Get the information about the investigator we're trying to edit
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
            
            //Pass those information to the view
            return View(m);
        }

        //Get the edited investagator input
        [HttpPost]
        public ActionResult EditInvestigator(EditInvestigatorViewModel m)
        {
            //Get the investigator we're trying to edit
            var investigator = _db.Investigators.FirstOrDefault(i => i.Id == m.Id);

            //If investigator exists in the database
            if (investigator != null)
            {
                //Edit the investigator details in the database
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

            //Show the admin dashboard
            return View("Index");
        }

        //Show the form to assign investigator to trial
        public ActionResult AssignInvestigatorToTrial()
        {
            //Get the lists of investigator and trial, and send it off to the view
            var m = new AssignInvestigatorToTrialViewModel();

            m.Investigators = _db.Investigators.OrderBy(i => i.Person.First_Name).ToArray();
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();

            return View(m);
        }

        //Get the input from the form and add it to the database
        [HttpPost]
        public ActionResult AssignInvestigatorToTrial(AssignInvestigatorToTrialViewModel m)
        {
            //Create trial investigator object and add that to the database
            var trialInvestigator = new PX_Model.Trial_Investigator();

            trialInvestigator.Investigator_Id = m.InvestigatorId;
            trialInvestigator.Trial_Id = m.TrialId;
            trialInvestigator.Type = m.Type;

            _db.Trial_Investigator.Add(trialInvestigator);
            _db.SaveChanges();

            return View("Index");
        }

        //Show the form to create baseline assessment
        public ActionResult CreateBaselineAssessment()
        {
            var m = new CreateBaselineAssessmentViewModel();
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            return View(m);
        }

        //Get the information to create the baseline assessment
        [HttpPost]
        public ActionResult CreateBaselineAssessment(CreateBaselineAssessmentViewModel m)
        {
            //If input is valid
            if (ModelState.IsValid)
            {
                //Create the assessment in the database
                var assessmentType = new PX_Model.Assessment_Type();
                assessmentType.Name = m.Name;
                assessmentType.Trial_Id = m.TrialId;
                assessmentType.Description = m.Description;
                _db.Assessment_Type.Add(assessmentType);
                _db.SaveChanges();

                //Create the question in the database linked with the assessment
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
                            //Save the options in the database
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
                //Go to admin dashboard once succeeded
                return View("Index");
            }
            //Get the trial list again and show the form again if there was an error
            m.Trials = _db.Trials.OrderBy(t => t.Name).ToArray();
            return View(m);
        }

        //Get the list of non randomised trial
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

        //Show the form to randomise trial
        public ActionResult RandomiseTrialParticipants()
        {
            //Get all trials that were not randomised
            var trialList = getNonRandomisedTrials();

            //If all trials were already randomised
            if (trialList.Count == 0)
            {
                ModelState.AddModelError("", "There are no trials that needs to be randomised");
                return View("Index");
            }

            var m = new RandomiseViewModel();
            m.Trials = trialList.AsEnumerable();

            return View(m);
        }

        //Randomise a trial
        [HttpPost]
        public ActionResult RandomiseTrialParticipants(Professional_Experience.Models.RandomiseViewModel m)
        {
            //Get the trial from database
            var trial = _db.Trials.FirstOrDefault(t => t.Id == m.TrialId);

            //If trial isn't in the database
            if (trial == null)
            {
                ModelState.AddModelError("", "Trial does not exist");
            }

            //If input had error
            if (m.NumberOfParticipantInIntervention < 0 || trial.Trial_Participant.Count < m.NumberOfParticipantInIntervention)
            {
                ModelState.AddModelError("", "Invalid number of participants to be added in intervention group");
            }

            //If everything is fine
            if (ModelState.IsValid)
            {
                var trialParticipants = trial.Trial_Participant;

                Random rnd = new Random();
                int count = trialParticipants.Count;
                int random = rnd.Next(0, count);
                
                //Randomly select the middle point in the list and divide the list into two groups
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

            //If there was an error
            m.Trials = getNonRandomisedTrials().AsEnumerable();
            return View(m);
        }

        //Show the list of all the trials
        public ActionResult ViewTrials(int pageRandomised = 1, int pageNonRandomised = 1)
        {
            var m = new Professional_Experience.Models.AdminViewTrialsViewModel();
            m.Randomised = new PagedList<PX_Model.Trial>(_db.Trials.Where(t => t.HasBeenRandomised == true).OrderBy(t => t.Name), pageRandomised, 7);
            m.NonRandomised = new PagedList<PX_Model.Trial>(_db.Trials.Where(t => t.HasBeenRandomised == false).OrderBy(t => t.Name), pageNonRandomised, 7);
            return View(m);
        }

        //Search trial according to intervention or experimental classification
        public ActionResult SearchTrialsAccordingToClassification(string searchWord)
        {
            searchWord = searchWord.ToLower();
            //Search for the search word in name and description of the trial
            var trials = _db.Trials.Where(t => t.Name.ToLower().Contains(searchWord) || t.Description.ToLower().Contains(searchWord)).OrderBy(t => t.Name);
            var m = new Professional_Experience.Models.AdminViewTrialsViewModel();
            m.Randomised = new PagedList<PX_Model.Trial>(trials.Where(t => t.HasBeenRandomised == true), 1, trials.Count() + 1);
            m.NonRandomised = new PagedList<PX_Model.Trial>(trials.Where(t => t.HasBeenRandomised == false), 1, trials.Count() + 1);
            return View("ViewTrials", m);
        }

        //Show the information about randomised trial
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

        //Show the information about non randomised trial
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

        //Get list of all the participants in a trial
        public ActionResult ViewAllParticipants(int id, int page = 1)
        {
            var trialParticipants = _db.Trial_Participant.
                Where(tp => tp.Trial_Id == id).OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            var m = new PagedList<PX_Model.Trial_Participant>(trialParticipants, page, 15);
            ViewBag.contentType = 0;
            ViewBag.Id = id;
            return View(m);
        }

        //Get a list of all the intervention participants in a trial
        public ActionResult ViewAllInterventionParticipant(int id, int page = 1)
        {
            var trialParticipants = _db.Trial_Participant.
                Where(tp => tp.Trial_Id == id && tp.Classification == PX_Model.Trial_Participant.CLASSIFICATION_INTERVENTION).
                OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            var m = new PagedList<PX_Model.Trial_Participant>(trialParticipants, page, 15);
            ViewBag.contentType = 1;
            ViewBag.Id = id;
            return View("ViewAllParticipants", m);
        }

        //Get a list of all the experimental participants in a trial
        public ActionResult ViewAllExperimentalParticipant(int id, int page = 1)
        {
            var trialParticipants = _db.Trial_Participant.
                Where(tp => tp.Trial_Id == id && tp.Classification == PX_Model.Trial_Participant.CLASSIFICATION_CONTROL).
                OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            var m = new PagedList<PX_Model.Trial_Participant>(trialParticipants, page, 15);
            ViewBag.contentType = 2;
            ViewBag.Id = id;
            return View("ViewAllParticipants", m);
        }

        //Search for participant in a trial
        [HttpPost]
        public ActionResult SearchAllParticipants(int id, int type, string searchWord)
        {
            IEnumerable<PX_Model.Trial_Participant> trialParticipants;
            searchWord = searchWord.ToLower();

            if (type == 0)
            {
                //All participant regardless of their classification
                trialParticipants = _db.Trial_Participant.
                    Where(tp => tp.Trial_Id == id).
                    OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            }
            else
            {
                //Participants based on their classification
                trialParticipants = _db.Trial_Participant.
                    Where(tp => tp.Trial_Id == id && tp.Classification == type - 1).
                    OrderBy(tp => tp.Participant.Person.First_Name).AsEnumerable();
            }

            //Search through the list for search words in first name, last name and username
            var searchParticipants = trialParticipants.Where(tp => tp.Participant.Person.First_Name.ToLower().Contains(searchWord)
                || tp.Participant.Person.Last_Name.ToLower().Contains(searchWord) 
                || tp.Participant.Person.Username.ToLower().Contains(searchWord));

            var m = new PagedList<PX_Model.Trial_Participant>(searchParticipants, 1, searchParticipants.Count() + 1);
            ViewBag.contentType = type;
            ViewBag.Id = id;
            return View("ViewAllParticipants", m);
        }

        //Shows participants that are part of certian group
        public ActionResult ViewParticipantsInGroup(int id, int pgid)
        {
            var m = _db.Trial_Participant.Where(tp => tp.Id == id && tp.Trial_Participant_Participant_Group.Where(tppg => tppg.Participant_Group.Id == pgid).Count() > 0);
            return View("ViewAllParticipants", new PagedList<PX_Model.Trial_Participant>(m, 1, m.Count() + 1));
        }

        //Shows the information of the participant
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

        //Shows reporting section
        public ActionResult Report()
        {
            return View(_db.Trials.OrderBy(t => t.Name).AsEnumerable());
        }

        //Creates and displays general report
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

            //Create bar chart and save it as an image file
            var barChart = new Chart(width: 750, height: 500)
                .AddTitle("Number of participant on trials")
                .AddSeries(
                    name: "Trials",
                    xValue: trialNames.ToArray(),
                    yValues: numberOfParticipants.ToArray());
            barChart.Save("~/Charts/bar.jpg");

            return View(trials);
        }

        //Shows individual trial report
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

            var xAxisValues = new List<string>();
            var yAxisValues = new List<int>();

            if (m.NumberOfMales != 0)
            {
                xAxisValues.Add("Male");
                yAxisValues.Add(m.NumberOfMales);
            }
            if (m.NumberOfFemales != 0)
            {
                xAxisValues.Add("Female");
                yAxisValues.Add(m.NumberOfFemales);
            }

            //Create gender pie char
            var genderChart = new Chart(width: 600, height: 400)
                .AddTitle("Number of participants according to gender")
                .AddSeries(
                    chartType: "Pie",
                    name: "Trials",
                    xValue: xAxisValues.ToArray(),
                    yValues: yAxisValues.ToArray());
            genderChart.Save("~/Charts/gender.jpg");

            xAxisValues.Clear();
            yAxisValues.Clear();
            if (m.NumberOfInterventionParticipant != 0)
            {
                xAxisValues.Add("Intervention");
                yAxisValues.Add(m.NumberOfInterventionParticipant);
            }
            if (m.NumberOfExperimentalParticipant != 0)
            {
                xAxisValues.Add("Experimental");
                yAxisValues.Add(m.NumberOfExperimentalParticipant);
            }

            //Create classification pie chart
            var classificationChart = new Chart(width: 600, height: 400)
                .AddTitle("Number of participants according to classification")
                .AddSeries(
                    chartType: "Pie",
                    name: "Trials",
                    xValue: xAxisValues.ToArray(),
                    yValues: yAxisValues.ToArray());
            classificationChart.Save("~/Charts/classification.jpg");

            m.BaselineAssessments = new List<TrialReportViewModel.BaselineAssessment>();
            var baselineAssessments = _db.Assessment_Type.Where(ba => ba.Trial_Id == id);

            //Calculate the percentage for each options chosen by the participant while
            //taking the baseline assessment
            foreach (var baselineAssessment in baselineAssessments)
            {
                var baseline = new TrialReportViewModel.BaselineAssessment();
                baseline.Name = baselineAssessment.Name;
                baseline.Questions = new List<TrialReportViewModel.Question>();
                var questions = baselineAssessment.Assessment_Type_Question.Where(q => q.Question_Type != 0);
                foreach (var question in questions) {
                    var options = new List<TrialReportViewModel.Option>();
                    double count = question.Assessment_Type_Question_Answer.Count;
                    foreach (var option in question.Assessment_Type_Option)
                    {
                        var opt = new TrialReportViewModel.Option();
                        opt.Opt = option.Opt;
                        opt.Percentage = ((double)question.Assessment_Type_Question_Answer.Where(qa => qa.Answer == option.Opt).Count() / count) * 100.0;
                        options.Add(opt);
                    }
                    var q = new TrialReportViewModel.Question();
                    q.Q = question.Question;
                    q.Options = options;
                    baseline.Questions.Add(q);
                }
                if (questions.Count() != 0)
                {
                    m.BaselineAssessments.Add(baseline);
                }
            }

            return View(m);
        }

        //Show clinician section
        public ActionResult Clinician()
        {
            return View();
        }

        //Show create clinician form
        public ActionResult CreateClinician()
        {
            return View();
        }

        //Get form input for create clinician and save that in database
        [HttpPost]
        public async Task<ActionResult> CreateClinician(CreateClinicianViewModel m)
        {
            //If input is valid
            if (ModelState.IsValid)
            {
                //Create login account
                var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = new ApplicationUser { UserName = m.Username, Email = m.Email };
                var result = await UserManager.CreateAsync(user, m.Password);
               
                //If creation of login account was a success
                if (result.Succeeded)
                {
                    //Create a person in the database
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

                    //Create clinician in the database
                    var clinician = new PX_Model.Clinician();

                    clinician.Person_Id = person.Id;

                    _db.Clinicians.Add(clinician);
                    _db.SaveChanges();

                    //Assign clinician account to clinician role
                    var u = UserManager.FindByName(m.Username);
                    UserManager.AddToRole(u.Id, "Clinician");

                    return View("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(m);
        }

        //Show list of clinicians
        public ActionResult EditClinicians(int page = 1)
        {
            var m = _db.Clinicians.OrderBy(c => c.Person.First_Name).AsEnumerable();
            return View(new PagedList<PX_Model.Clinician>(m, page, 10));
        }

        //Show the form to edit clinician
        public ActionResult EditClinician(int id)
        {
            //Get the clinician we want to edit
            var clinician = _db.Clinicians.FirstOrDefault(c => c.Id == id);
            if (clinician == null)
            {
                ModelState.AddModelError("", "Clinician not found");
                return View("Index");
            }

            //Extract the information from the database
            var m = new EditClinicianViewModel();

            m.Id = clinician.Id;
            m.FirstName = clinician.Person.First_Name;
            m.Email = clinician.Person.Email;
            m.LastName = clinician.Person.Last_Name;
            m.PhoneNumber = clinician.Person.Phone_Number;
            m.Postcode = clinician.Person.Postcode;
            m.State = clinician.Person.State;
            m.Street = clinician.Person.Street;
            m.Suburb = clinician.Person.Suburb;

            //Pass on the information to the view
            return View(m);
        }

        //Get the input for editing the clinician
        [HttpPost]
        public ActionResult EditClinician(EditClinicianViewModel m)
        {
            //Get the clinician we want to edit
            var clinician = _db.Clinicians.FirstOrDefault(c => c.Id == m.Id);
            if (clinician == null)
            {
                ModelState.AddModelError("", "Clinician not found");
                return View("Index");
            }

            //If input is valid
            if (ModelState.IsValid)
            {
                //Edit the clinician information in the database
                clinician.Person.Email = m.Email;
                clinician.Person.First_Name = m.FirstName;
                clinician.Person.Last_Name = m.LastName;
                clinician.Person.Phone_Number = m.PhoneNumber;
                clinician.Person.Postcode = m.Postcode;
                clinician.Person.State = m.State;
                clinician.Person.Street = m.Street;
                clinician.Person.Suburb = m.Suburb;

                _db.SaveChanges();
                return View("Index");
            }

            return View(m);
        }

        //Search for clinician
        public ActionResult SearchClinicians(string searchWord)
        {
            searchWord = searchWord.ToLower();
            //Search for search word in Name and username
            var clinicians = _db.Clinicians.Where(c => c.Person.First_Name.ToLower().Contains(searchWord)
                || c.Person.Last_Name.ToLower().Contains(searchWord)
                || c.Person.Username.ToLower().Contains(searchWord));
            return View("EditClinicians", new PagedList<PX_Model.Clinician>(clinicians.OrderBy(c => c.Person.First_Name), 1, clinicians.Count() + 1));
        }
    }
}