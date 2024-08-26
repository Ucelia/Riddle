using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace RiddelMyExam.Pages
{
    public class RiddleLdPgeModel : PageModel
    {
        public readonly IConfiguration Configuration;
        public List<riddle> riddlelist = new List<riddle>();
        public void OnGet()
        {
           // GetRiddlesFromDb();
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
                ErrorMessage = ex.Message;
            }
        }

    }
}
