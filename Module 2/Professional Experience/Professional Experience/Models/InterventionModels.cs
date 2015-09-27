using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class InterventionModels
    {
        public class Intervention
        {
            public Intervention(String name, String description, List<int> investigators, List<Test> tests)
            {
                Intervention_Name = name;
                Intervention_Description = description;
                Investigators = investigators;
                Tests = tests;
            }
            public String Intervention_Name { get; protected set; }
            public String Intervention_Description { get; protected set; }
            public List<int> Investigators { get; protected set; }
            public virtual List<Test> Tests { get; protected set; }
        }

        public class Test
        {
            public Test(int id, String name, String description, List<Question> questions)
            {
                Test_Id = id;
                Test_Name = name;
                Test_Description = description;
                Questions = questions;
            }
            public int Test_Id { get; protected set; }
            public String Test_Name { get; protected set; }
            public String Test_Description { get; protected set; }
            public virtual List<Question> Questions { get; protected set; }
        }

        public class Question
        {
            public Question(int id, String title, int answerType, List<String> answers)
            {
                Question_Id = id;
                Question_Title = title;
                Answer_Type = answerType;
                Answers = answers;
            }
            public int Question_Id { get; protected set; }
            public String Question_Title { get; protected set; }
            public int Answer_Type { get; protected set; }
            public List<String> Answers { get; protected set; }
        }
    }
}