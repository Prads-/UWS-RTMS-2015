using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

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

        public ActionResult ParticipateTrialList(int page = 1)
        {
            var participant = GetCurrentParticipant;
            var trials = _db.Trials.Where(t => t.Trial_Participant.Where(tp => tp.Participant_Id == participant.Id).Count() == 0);
            
            return View(new PagedList<PX_Model.Trial>(trials.OrderBy(t => t.Name), page, 5));
        }

        public ActionResult AcceptTermsAndConditions(int id)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == id);

            if (trial == null)
            {
                ModelState.AddModelError("", "Trial was not found!");
                return View("Index");
            }

            var m = new Professional_Experience.Models.TermsAndConditionViewModel();

            m.Id = id;
            m.TermsAndCondition = trial.TermsAndConditions;

            return View(m);
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

        public ActionResult MyTrials(int page = 1)
        {
            var participant = GetCurrentParticipant;
            var trials = _db.Trials.Where(t => t.Trial_Participant.Where(tp => tp.Participant_Id == participant.Id).Count() != 0);

            return View(new PagedList<PX_Model.Trial>(trials.OrderBy(t => t.Name), page, 5));
        }

        public ActionResult ViewTrial(int id)
        {
            var trial = _db.Trials.FirstOrDefault(t => t.Id == id);
            if (trial == null)
            {
                ModelState.AddModelError("", "Trail not found");
                return View("Index");
            }
            var m = new List<Professional_Experience.Models.ViewTrialViewModel>();

            if (trial.HasBeenRandomised == true)
            {
                var baselineAssessments = _db.Assessment_Type.Where(at => at.Trial_Id == id);
                int partcipantId = GetCurrentParticipant.Id;

                foreach (var ba in baselineAssessments)
                {
                    var tpba = _db.Trial_Participant_Assessment_Type.FirstOrDefault(t => t.Trial_Participant_Id == partcipantId && t.Assessment_Type_Id == ba.Id);
                    if (tpba == null)
                    {
                        var model = new Professional_Experience.Models.ViewTrialViewModel();
                        model.BaselineAssessmentId = ba.Id;
                        model.BaselineAssessmentDescription = ba.Description;
                        model.BaselineAssessmentName = ba.Name;
                        m.Add(model);
                    }
                }
            }

            return View(m.AsEnumerable());
        }

        public ActionResult TakeBaselineAssessment(int id)
        {
            var baselineAssessment = _db.Assessment_Type.FirstOrDefault(ba => ba.Id == id);
            if (baselineAssessment == null)
            {
                ModelState.AddModelError("", "Baseline assessment not found in the database!");
                return View("Index");
            }
            var questions = baselineAssessment.Assessment_Type_Question;
            var m = new Professional_Experience.Models.TakeBaselineAssessmentViewModel();
            m.AssessmentTypeId = baselineAssessment.Id;
            m.Answers = new List<Models.AnswerBaselineAssessmentQuestionViewModel>();

            foreach (var question in questions)
            {
                var q = new Professional_Experience.Models.AnswerBaselineAssessmentQuestionViewModel();
                q.Question = question.Question;
                q.QuestionId = question.Id;
                q.QuestionType = (int)question.Question_Type;

                if (question.Assessment_Type_Option != null)
                {
                    q.Options = new List<string>();
                    foreach (var option in question.Assessment_Type_Option)
                    {
                        q.Options.Add(option.Opt);
                    }
                }
                m.Answers.Add(q);
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult TakeBaselineAssessment(Professional_Experience.Models.TakeBaselineAssessmentViewModel m)
        {
            var trialParticipantAssessmentType = new PX_Model.Trial_Participant_Assessment_Type();
            trialParticipantAssessmentType.Assessment_Type_Id = m.AssessmentTypeId;
            trialParticipantAssessmentType.Trial_Participant_Id = GetCurrentParticipant.Id;
            _db.Trial_Participant_Assessment_Type.Add(trialParticipantAssessmentType);
            _db.SaveChanges();

            foreach (var answer in m.Answers)
            {
                if (answer.QuestionType == PX_Model.Assessment_Type_Question.TYPE_MULTU_CHOICE_MULTI_SELECT)
                {
                    foreach (var a in answer.Options)
                    {
                        var ans = new PX_Model.Assessment_Type_Question_Answer();
                        ans.Answer = a;
                        ans.Answer_Type = answer.QuestionType;
                        ans.Assessment_Type_Question_Id = answer.QuestionId;
                        ans.Trial_Participant_Assessment_Type_Id = trialParticipantAssessmentType.Id;
                        _db.Assessment_Type_Question_Answer.Add(ans);
                    }
                }
                else
                {
                    var ans = new PX_Model.Assessment_Type_Question_Answer();
                    ans.Answer = answer.Answer;
                    ans.Answer_Type = answer.QuestionType;
                    ans.Assessment_Type_Question_Id = answer.QuestionId;
                    ans.Trial_Participant_Assessment_Type_Id = trialParticipantAssessmentType.Id;
                    _db.Assessment_Type_Question_Answer.Add(ans);
                }
                _db.SaveChanges();
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult SearchTrials(string searchWord, bool myTrials) 
        {
            var participant = GetCurrentParticipant;
            IEnumerable<PX_Model.Trial> trials;
            searchWord = searchWord.ToLower();

            if (myTrials)
            {
                trials = _db.Trials.Where(t => t.Trial_Participant.Where(tp => tp.Participant_Id == participant.Id).Count() != 0);
            }
            else
            {
                trials = _db.Trials.Where(t => t.Trial_Participant.Where(tp => tp.Participant_Id == participant.Id).Count() == 0);
            }

            trials = trials.Where(t => t.Name.ToLower().Contains(searchWord) || t.Description.ToLower().Contains(searchWord));
            var m = new PagedList<PX_Model.Trial>(trials.OrderBy(t => t.Name), 1, trials.Count() + 1);
            
            return ((myTrials) ? View("MyTrials", m) : View("ParticipateTrialList", m));
        }
    }
}