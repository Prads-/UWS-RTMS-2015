using Professional_Experience.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
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

        public ActionResult InterventionResults()
        {
            String username = System.Web.HttpContext.Current.User.Identity.Name;
            String connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            String sql = "SELECT * FROM Trial INNER JOIN Trial_Participant ON Trial_Participant.Trial_Id = Trial.Id INNER JOIN Participant ON Trial_Participant.Participant_Id = Participant.Id INNER JOIN Person ON Participant.Person_Id = Person.Id AND Person.Username = '" + username + "';";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader nwReader = cmd.ExecuteReader();
            while (nwReader.Read())
            {
                ViewBag.TrialName = nwReader["Name"].ToString();
                ViewBag.TrialDescription = nwReader["Description"].ToString();
                ViewBag.TrialStartDate = nwReader["Start_Date"].ToString();
                ViewBag.TrialEndDate = nwReader["End_Date"].ToString();
            }
            nwReader.Close();
            List<InterventionModels.Intervention> interventions = new List<InterventionModels.Intervention>();
            sql = "SELECT * FROM Intervention_Area;";
            cmd = new SqlCommand(sql, conn);
            nwReader = cmd.ExecuteReader();
            while (nwReader.Read())
            {
                String interventionId = nwReader["Id"].ToString();
                String name = nwReader["Name"].ToString();
                String description = nwReader["Description"].ToString();
                List<InterventionModels.Test> tests = new List<InterventionModels.Test>();
                SqlConnection testConn = new SqlConnection(connectionString);
                testConn.Open();
                sql = "SELECT * FROM Intervention_Area_Test WHERE Intervention_Area_Id = '" + interventionId + "';";
                SqlCommand testCmd = new SqlCommand(sql, testConn);
                SqlDataReader testReader = testCmd.ExecuteReader();
                while (testReader.Read())
                {
                    int testId = Convert.ToInt32(testReader["Id"].ToString());
                    String testName = testReader["Name"].ToString();
                    String testDescription = testReader["Description"].ToString();
                    tests.Add(new InterventionModels.Test(testId, testName, testDescription, null));
                }
                interventions.Add(new InterventionModels.Intervention(name, description, null, tests));
                testReader.Close();
                testConn.Close();
            }
            nwReader.Close();
            conn.Close();

            ViewBag.Username = username;
            ViewBag.Interventions = interventions;
            return View();
        }

        public ActionResult CompleteTest(String testName, int testId)
        {
            List<InterventionModels.Question> questions = new List<InterventionModels.Question>();
            String connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            String sql = "SELECT * FROM Intervention_Area_Test_Question WHERE Intervention_Area_Test_Id = '" + testId + "';";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader nwReader = cmd.ExecuteReader();
            while (nwReader.Read())
            {
                int questionId = Convert.ToInt32(nwReader["Id"].ToString());
                String questionName = nwReader["Question"].ToString();
                int answerType = Convert.ToInt32(nwReader["Question_Type"].ToString());
                List<String> answers = new List<String>();
                SqlConnection answerConn = new SqlConnection(connectionString);
                answerConn.Open();
                sql = "SELECT * FROM Intervention_Area_Test_Question_Option WHERE Intervention_Area_Test_Question_Id = '" + questionId + "';";
                SqlCommand answerCmd = new SqlCommand(sql, answerConn);
                SqlDataReader answerReader = answerCmd.ExecuteReader();
                while (answerReader.Read())
                {
                    String answer = answerReader["Opt"].ToString();
                    answers.Add(answer);
                }
                questions.Add(new InterventionModels.Question(questionId, questionName, answerType, answers));
                answerReader.Close();
                answerConn.Close();
            }
            nwReader.Close();
            conn.Close();
            ViewBag.Questions = questions;
            ViewBag.TestName = testName;
            ViewBag.TestId = testId;
            
            return View();
        }


    }
}