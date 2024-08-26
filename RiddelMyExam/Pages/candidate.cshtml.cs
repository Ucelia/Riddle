using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace RiddelMyExam.Pages
{
    public class candidateModel : PageModel
    {
        private readonly IConfiguration Configuration;

        public candidateModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public List<questionM> Questions { get; set; }
        public string ErrorMessage { get; set; }
        public int CurrentQuestionIndex { get; set; } = 0;
        public int anseww;
        public void OnGet()
        {
            GetQuestionsFromDb();
        }
        public int question_id;
        public int data;
        int answ;
        int score;
        public void OnPost()
        {
            OnGet();

            data = Int32.Parse(Request.Form["answer"]);
            answ = Int32.Parse(Request.Form["correct"]);
            int user_id = Convert.ToInt32(HttpContext.Session.GetInt32("_id"));
            question_id = Int32.Parse(Request.Form["questionIDD"]);

            if (data == answ)
            {
                score += 1;
            }
            else
            {
                ErrorMessage = "Select your answer!!";
            }

            ErrorMessage = "" + score;

            String? conn = Configuration.GetConnectionString("con");
            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    String qry = "insert into Report values (@user_id,@question_id,@marks)";
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        cmd.Parameters.AddWithValue("@question_id", question_id);
                        cmd.Parameters.AddWithValue("@marks", score);

                        score = 0;
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ErrorMessage = "Saved Successfully";
                        }
                        else
                        {
                            ErrorMessage = "Failed to Save";
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }


        }
    
        public IActionResult OnPostGeneratePdf()
        {
            GetQuestionsFromDb();

            var pdfBytes = GeneratePdf(Questions);

            return File(pdfBytes, "application/pdf", "Questions.pdf");
        }

       
        private void GetQuestionsFromDb()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("con");

                int user_id = Convert.ToInt32(HttpContext.Session.GetInt32("_id"));

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM questions " +
                                   "WHERE id NOT IN (SELECT question FROM Report WHERE user_id = @user_id)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", user_id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Questions = new List<questionM>();

                            while (reader.Read())
                            {
                                var question = new questionM
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    qst = reader.GetString(reader.GetOrdinal("question")),
                                    opt1 = reader.GetString(reader.GetOrdinal("option1")),
                                    opt2 = reader.GetString(reader.GetOrdinal("option2")),
                                    opt3 = reader.GetString(reader.GetOrdinal("option3")),
                                    correct = reader.GetInt32(reader.GetOrdinal("correct"))
                                };

                                Questions.Add(question);
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


        private byte[] GeneratePdf(List<questionM> questions)
        {
            var pdfStream = new MemoryStream();

            if (questions != null && questions.Count > 0)
            {
                try
                {
                    using (var document = new Document())
                    {
                        using (var writer = PdfWriter.GetInstance(document, pdfStream))
                        {
                            document.Open();

                            foreach (var question in questions)
                            {
                                
                                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                                var title = new Paragraph(question.qst, titleFont);
                                title.Alignment = Element.ALIGN_CENTER;
                                document.Add(title);

                                
                                document.Add(new Paragraph("\n"));

                                
                                var optionFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                                document.Add(new Paragraph($"Option 1: {question.opt1}", optionFont));
                                document.Add(new Paragraph($"Option 2: {question.opt2}", optionFont));
                                document.Add(new Paragraph($"Option 3: {question.opt3}", optionFont));

                               
                                document.Add(new Paragraph("\n"));

                                document.Add(new LineSeparator());

                                document.Add(new Paragraph("\n"));
                            }

                            document.Close();
                        }
                    }

                    pdfStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }

            return pdfStream.ToArray();
        }

        public class questionM
        {
            public int? id { get; set; }
            public string? rid_id { get; set; }
            public string? qst { get; set; }
            public string? opt1 { get; set; }
            public string? opt2 { get; set; }
            public string? opt3 { get; set; }
            public int? correct { get; set; }
        }
        public class certifi
        {
            public int? id { get; set;}
            public int? marks { get; set; }
            public string? qstione_name { get; set; }
        
        }
    }
}
