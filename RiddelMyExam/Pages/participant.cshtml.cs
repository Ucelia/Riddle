using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RiddelMyExam.Pages
{
    public class participantModel : PageModel
    {
     
        private readonly IConfiguration Configuration;

        public participantModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public question1 qstt = new question1();
        public sampling sampling = new sampling();
        public List<question1> Questions { get; set; }
        public string ErrorMessage = "";
        public int CurrentQuestionIndex { get; set; } = 0;
        public void OnGet(int questionIndex)
        {
            CurrentQuestionIndex = questionIndex;
            GetQuestionsFromDb();
        }
        public void OnPost()
        {
            qstt.correct = Request.Form["correct"];
            
            string userAnswer = Request.Form["correct"];
            sampling.answer= Request.Form["correct"];


            if (Questions != null && CurrentQuestionIndex < Questions.Count)
            {
                string correctAnswer = Questions[CurrentQuestionIndex].correct;

                
                bool isAnswerCorrect = string.Equals(userAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);

                
                InsertUserAnswerIntoDatabase((int)Questions[CurrentQuestionIndex].id, userAnswer);

                if (isAnswerCorrect)
                {
                    
                    OnPostNextQuestion();
                }
                else
                {
                    ErrorMessage = "Incorrect answer. Please try again.";
                    
                }
            }


        }
      

        public IActionResult OnPostNextQuestion()
        {
            CurrentQuestionIndex++;


            if (Questions != null && CurrentQuestionIndex < Questions.Count)
            {
                return Page();
            }
            else
            {

                GetQuestionsFromDb();

                if (Questions != null && Questions.Count > 0)
                {

                    return Page();
                }
                else
                {
                    return RedirectToPage("/SummaryPage");
                }
            }

        }
        private void InsertUserAnswerIntoDatabase(int questionId, string userAnswer)
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("con");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO sample VALUES (@QuestionId, @UserAnswer)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@QuestionId", sampling.qstId);
                        command.Parameters.AddWithValue("@UserAnswer", sampling.answer);


                        
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                
            }
        }

            private void GetQuestionsFromDb()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("con");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM questionA"; 
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Questions = new List<question1>();

                            while (reader.Read())
                            {

                                {
                                    question1 qst = new question1();
                                   
                                    qst.id = reader.GetInt32(reader.GetOrdinal("id"));
                                
                                    qst.qst = reader.GetString(reader.GetOrdinal("question"));
                                 
                                    qst.correct = reader.GetString(reader.GetOrdinal("correct"));
                                    Questions.Add(qst);
                                };


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
    public class question1
    {
        public int? id { get; set; }
        public string? rid_id { get; set; }
        public string? qst { get; set; }
    
        public string? correct { get; set; }

  
}
}
public class sampling
{
    public int? id { get; set; }
    public int? qstId { get; set; }
    public string? answer { get; set; }
}
