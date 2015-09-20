using Newtonsoft.Json;
using Professional_Experience.Models;
using PX_Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Professional_Experience.Controllers
{
   [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        // GET: Administrator
        public ActionResult Index()
        {
            return View();
        }

       public ActionResult InterventionSetup()
        {
           
            return View();
        }
        public ActionResult Reporting()
        {
            return View();
        }

        struct MyObj
        {
            public string test { get; set; }
        }
        [HttpPost]
        public String SubmitIntervention()
        {
            HttpContext.Request.InputStream.Position = 0;
            var result = new System.IO.StreamReader(HttpContext.Request.InputStream).ReadToEnd().ToString();
            dynamic intervention = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

            if (validIntervention(intervention))
            {
                insertIntervention(intervention);
                return "The intervention was successfully created";
            }
            else
            {
                return "The intervention was not created due to validation errors!";
            }
        }
        public string GetInvestigators()
        {
            String connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            String sql = "SELECT Person.First_Name, Person.Last_Name, Investigator.Id, Investigator.Institution FROM Investigator INNER JOIN Person ON Investigator.Person_id = Person.Id;";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return ConvertDataTabletoString(dt);
        }

        private void insertIntervention(dynamic intervention){
            String connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            String sql = "INSERT INTO Intervention_Area (Name, Description) OUTPUT INSERTED.ID values (@Intervention_Name, @Intervention_Description)";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Intervention_Name", SqlDbType.VarChar).Value = intervention.Intervention_Name;
            cmd.Parameters.Add("@Intervention_Description", SqlDbType.VarChar).Value = intervention.Intervention_Description;
            int interventionId = (int)cmd.ExecuteScalar();
            int[] Investigators = intervention.Investigators.ToObject<int[]>();
            for (int i = 0; i < Investigators.Length; i++) //insert investigators
            {
                sql = "INSERT INTO Investigator_Intervention_Area (Investigator_Id, Intervention_Area_Id) values (@Investigator_Id, @Intervention_Area_Id)";
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Investigator_Id", SqlDbType.Int).Value = Investigators[i];
                cmd.Parameters.Add("@Intervention_Area_Id", SqlDbType.Int).Value = interventionId;
                cmd.ExecuteNonQuery();
            }
            dynamic[] Tests = intervention.Tests.ToObject<dynamic[]>();
            for (int i = 0; i < Tests.Length; i++)
            {
                sql = "INSERT INTO Intervention_Area_Test (Intervention_Area_Id, Name, Description) OUTPUT INSERTED.ID values (@Intervention_Area_Id, @Test_Name, @Test_Description)";
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@Intervention_Area_Id", SqlDbType.Int).Value = interventionId;
                cmd.Parameters.Add("@Test_Name", SqlDbType.VarChar).Value = Tests[i].Test_Name;
                cmd.Parameters.Add("@Test_Description", SqlDbType.VarChar).Value = Tests[i].Test_Description;
                int testId = (int)cmd.ExecuteScalar();
                dynamic[] Questions = intervention.Tests[i].Questions.ToObject<dynamic[]>();
                for (int j = 0; j < Questions.Length; j++)
                {
                    sql = "INSERT INTO Intervention_Area_Test_Question (Intervention_Area_Test_Id, Question, Question_Type) OUTPUT INSERTED.ID values (@Intervention_Area_Test_Id, @Question, @Question_Type)";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add("@Intervention_Area_Test_Id", SqlDbType.Int).Value = testId;
                    cmd.Parameters.Add("@Question", SqlDbType.VarChar).Value = Questions[j].Question_Title;
                    cmd.Parameters.Add("@Question_Type", SqlDbType.VarChar).Value = Questions[j].Answer_Type;
                    int questionId = (int)cmd.ExecuteScalar();
                    dynamic[] Answers = intervention.Tests[i].Questions[j].Answers.ToObject<dynamic[]>();
                    for(int k = 0; k < Answers.Length; k++)
                    {
                        sql = "INSERT INTO Intervention_Area_Test_Question_Option (Intervention_Area_Test_Question_Id, Opt) values (@Intervention_Area_Test_Question_Id, @Opt)";
                        cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.Add("@Intervention_Area_Test_Question_Id", SqlDbType.Int).Value = questionId;
                        cmd.Parameters.Add("@Opt", SqlDbType.VarChar).Value = Answers[k];
                        cmd.ExecuteNonQuery();
                    }
                }
            }
                conn.Close();
        }
        private bool validIntervention(dynamic intervention)
        {
            bool validForm = true;
            if (validForm)
                return true;
            else
                return false;
        }
        public string ConvertDataTabletoString(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
    }
}