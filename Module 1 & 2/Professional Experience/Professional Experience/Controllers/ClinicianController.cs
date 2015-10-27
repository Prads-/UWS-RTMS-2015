using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Professional_Experience.Controllers
{
    [Authorize(Roles = "Clinician")]
    public class ClinicianController : UIController
    {
        private int MyId
        {
            get
            {
                string username = User.Identity.Name;
                var person = _db.People.FirstOrDefault(p => p.Username == username);
                return person.Clinicians.ElementAt(0).Id;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ParticipantList(int page = 1)
        {
            var participants = _db.Participants.Where(p => p.Participant_Clinician.FirstOrDefault(pc => pc.Clinician_Id == MyId && pc.Participant_Id == p.Id) == null);
            return View(new PagedList<PX_Model.Participant>(participants.OrderBy(p => p.Person.First_Name), page, 10));
        }

        public ActionResult MyParticipantList(int page = 1)
        {
            var participants = _db.Participants.Where(p => p.Participant_Clinician.FirstOrDefault(pc => pc.Clinician_Id == MyId && pc.Participant_Id == p.Id) != null);
            return View(new PagedList<PX_Model.Participant>(participants.OrderBy(p => p.Person.First_Name), page, 10));
        }

        public ActionResult AssignParticipant(int id)
        {
            var participant = _db.Participants.FirstOrDefault(p => p.Id == id);
            if (participant == null)
            {
                ModelState.AddModelError("", "Participant not found");
                return View("Index");
            }
            return View(participant);
        }

        public ActionResult ConnectParticipantToClinician(int id)
        {
            var participant = _db.Participants.FirstOrDefault(p => p.Id == id);
            if (participant == null)
            {
                ModelState.AddModelError("", "Participant not found");
                return View("Index");
            }

            var pc = new PX_Model.Participant_Clinician();
            pc.Participant_Id = id;
            pc.Clinician_Id = MyId;
            _db.Participant_Clinician.Add(pc);
            _db.SaveChanges();

            return View("Index");
        }

        public ActionResult SearchParticipant(string searchWord, bool myParticipants)
        {
            searchWord = searchWord.ToLower();
            IEnumerable<PX_Model.Participant> participants;
            if (myParticipants)
            {
                participants = _db.Participants.Where(p => p.Participant_Clinician.FirstOrDefault(pc => pc.Clinician_Id == MyId && pc.Participant_Id == p.Id) != null
                    && (p.Person.First_Name.ToLower().Contains(searchWord)
                    || p.Person.Last_Name.ToLower().Contains(searchWord)
                    || p.Person.Username.ToLower().Contains(searchWord)));
                return View("MyParticipantList", new PagedList<PX_Model.Participant>(participants.OrderBy(p => p.Person.First_Name), 1, participants.Count() + 1));
            }
            else
            {
                participants = _db.Participants.Where(p => p.Participant_Clinician.FirstOrDefault(pc => pc.Clinician_Id == MyId && pc.Participant_Id == p.Id) == null
                    && (p.Person.First_Name.ToLower().Contains(searchWord)
                    || p.Person.Last_Name.ToLower().Contains(searchWord)
                    || p.Person.Username.ToLower().Contains(searchWord)));
                return View("ParticipantList", new PagedList<PX_Model.Participant>(participants.OrderBy(p => p.Person.First_Name), 1, participants.Count() + 1));
            }
        }

        public ActionResult MonitorParticipant(int id)
        {
            var participant = _db.Participants.FirstOrDefault(p => p.Id == id);
            if (participant == null)
            {
                ModelState.AddModelError("", "Participant not found");
                return View("Index");
            }
            return View(participant);
        }

        public ActionResult MonitorParticipantTrial(int pid, int tid)
        {
            var trialParticipant = _db.Trial_Participant.FirstOrDefault(tp => tp.Trial_Id == tid && tp.Participant_Id == pid);
            if (trialParticipant == null)
            {
                ModelState.AddModelError("", "Trial Participant not found");
                return View("Index");
            }

            var m = new Professional_Experience.Models.MonitorTrialParticipantViewModel();
            m.BaselineAssessments = new List<Models.MonitorTrialParticipantViewModel.BaselineAssessment>();
            m.Trial = trialParticipant.Trial;

            foreach (var tpba in trialParticipant.Trial_Participant_Assessment_Type)
            {
                var baselineAssessment = new Models.MonitorTrialParticipantViewModel.BaselineAssessment();
                baselineAssessment.Name = tpba.Assessment_Type.Name;
                baselineAssessment.Questions = new List<Models.MonitorTrialParticipantViewModel.Question>();
                foreach (var baq in tpba.Assessment_Type.Assessment_Type_Question)
                {
                    var question = new Models.MonitorTrialParticipantViewModel.Question();
                    question.Ques = baq.Question;
                    question.Answers = new List<string>();
                    var answers = tpba.Assessment_Type_Question_Answer.Where(atqa => atqa.Assessment_Type_Question_Id == baq.Id);
                    foreach (var answer in answers)
                    {
                        question.Answers.Add(answer.Answer);
                    }
                    baselineAssessment.Questions.Add(question);
                }
                m.BaselineAssessments.Add(baselineAssessment);
            }

            return View(m);
        }
    }
}