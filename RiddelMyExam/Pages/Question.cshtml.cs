using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace RiddelMyExam.Pages
{
    public class QuestionModel : PageModel
    {
        public readonly IConfiguration Configuration;
        public QuestionModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public quest qstn = new quest();
        public List<riddle> riddlelist = new List<riddle>();

        public string errorMessage = "";
        public void OnGet()
        {
            GetRiddlesFromDb();
        }
        public void OnPost()
        {
            qstn.rid_id = Request.Form["riddle_id"];
            qstn.qst = Request.Form["question"];
            qstn.opt1 = Request.Form["Option1"];
            qstn.opt2 = Request.Form["Option2"];
            qstn.opt3 = Request.Form["Option3"];
            qstn.correct = Request.Form["correct"];



            if (qstn.qst.Trim().Length == 0 || qstn.opt1.Trim().Length == 0 || qstn.opt2.Trim().Length == 0 || qstn.correct.Trim().Length == 0 || qstn.opt3.Trim().Length == 0)
            {

                errorMessage = "please fill in all the required fields";
            }
            else
            {
                string? constring = Configuration.GetConnectionString("con");
                try
                {
                    using (SqlConnection con = new SqlConnection(constring))
                    {

                        string query = " insert into questions values(@rid, @question, @opt1, @opt2, @opt3, @correct)";

                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {

                            cmd.Parameters.AddWithValue("@rid", qstn.rid_id);
                            cmd.Parameters.AddWithValue("@question", qstn.qst);
                            cmd.Parameters.AddWithValue("@opt1", qstn.opt1);
                            cmd.Parameters.AddWithValue("@opt2", qstn.opt2);
                            cmd.Parameters.AddWithValue("@opt3", qstn.opt3);
                            cmd.Parameters.AddWithValue("@correct", qstn.correct);
                            int rowAffected = cmd.ExecuteNonQuery();
                            if (rowAffected > 0)
                            {
                                errorMessage = "riddles registered";
                                //Response.Redirect("/login");
                            }
                            else
                            {
                                errorMessage = "Failed to register questions";
                            }
                        }
                        con.Close();
                    }
                }

                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

            }
        }

        private void GetRiddlesFromDb()
        {
            try
            {
                string connectionString = Configuration.GetConnectionString("con");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM riddle";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                {
                                    riddle r = new riddle();
                                    r.riddel_id = reader.GetString(reader.GetOrdinal("id"));
                                    r.riddel_name = reader.GetString(reader.GetOrdinal("name"));
                                    riddlelist.Add(r);
                                };

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}

public class quest
{
    public int? id { get; set; }
    public string? rid_id { get; set; }
    public string? qst { get; set; }
    public string? opt1 { get; set; }
    public string? opt2 { get; set; }
    public string? opt3 { get; set; }
    public string? correct { get; set; }

}
public class riddle
{
    public string riddel_name { get; set; }
    public string riddel_id { get; set; }
}