using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RiddelMyExam.Pages
{
    public class questioneModel : PageModel
    {
        public readonly IConfiguration Configuration;
        public questioneModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // string? conn = Configuration.GetConnectionString("con");
        public quests qstn = new quests();
        public List<riddles> riddlelist = new List<riddles>();

        public string errorMessage = "";
        public void OnGet()
        {
            GetRiddlesFromDb();
        }
        public void OnPost()
        {
            qstn.rid_id = Request.Form["riddle_id"];
            qstn.qst = Request.Form["question"];
           
            qstn.correct = Request.Form["correct"];



            if (qstn.qst.Trim().Length == 0  || qstn.correct.Trim().Length == 0)
            {

                errorMessage = "please fill in all required fields";
            }
            else
            {
                string? constring = Configuration.GetConnectionString("con");
                try
                {
                    //String conString = @"Data Source=DESKTOP-93JCHRB\\SQLEXPRESS;Initial Catalog=Riddles;Integrated Security=True";
                    using (SqlConnection con = new SqlConnection(constring))
                    {

                        string query = " insert into questionA values(@rid, @question, @correct)";

                        con.Open();
                        using (SqlCommand cmd = new(query, con))
                        {

                            cmd.Parameters.AddWithValue("@rid", qstn.rid_id);
                            cmd.Parameters.AddWithValue("@question", qstn.qst);
                           
                            cmd.Parameters.AddWithValue("@correct", qstn.correct);
                            int rowAffected = cmd.ExecuteNonQuery();
                            if (rowAffected > 0)
                            {
                                errorMessage = "riddle questions registered";
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
                                    riddles r = new riddles();
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

public class quests
{
    public int? id { get; set; }
    public string? rid_id { get; set; }
    public string? qst { get; set; }

    public string? correct { get; set; }

}
public class riddles
{
    public string riddel_name { get; set; }
    public string riddel_id { get; set; }
}
    

