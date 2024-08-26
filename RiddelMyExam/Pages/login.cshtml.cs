using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static RiddelMyExam.Pages.loginModel;

namespace RiddelMyExam.Pages
{
    public class loginModel : PageModel
    {

        public readonly IConfiguration Configuration;
        public loginModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public users log = new users();

        public string errorMessage = "";
        public void OnGet()
        {
        }
        public void OnPost()
        {

            log.email = Request.Form["email"];
            log.password = Request.Form["password"];

            if (string.IsNullOrEmpty(log.email) || string.IsNullOrEmpty(log.password))
            {
                errorMessage = "all fields are required";
            }
            else
            {
                String? conn = Configuration.GetConnectionString("con");
                try
                {
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        String qry = "select * from users where email=@email and password=@password";
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(qry, con))
                        {
                            cmd.Parameters.AddWithValue("@email", log.email);
                            cmd.Parameters.AddWithValue("@password", log.password);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                                if (reader.HasRows)
                                {
                                    if (reader.Read())
                                    {
                                        users logs = new users();
                                        logs.password = reader.GetString(3);
                                        logs.role = reader.GetString(4);
                                        HttpContext.Session.SetInt32("_id", Convert.ToInt32(reader["id"]));
                                        HttpContext.Session.SetString("_email", reader.GetString(2));
                                        HttpContext.Session.SetString("UserRole", reader.GetString(4));

                                        if (logs.password.Equals(log.password))
                                        {
                                            if (logs.role.Equals("Admin"))
                                            {
                                                errorMessage = "Welcome Admin";
                                                Response.Redirect("/Admin");
                                            }
                                            else
                                            {
                                                Response.Redirect("/candidate");
                                            }
                                        }
                                        else
                                        {
                                            errorMessage = "Invalid credentials";
                                        }
                                    }
                                }
                                con.Close();
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
    public class users
    {
        public int? id { get; set; }
        public String email { get; set; }
        public String password { get; set; }
        public String role { get; set; }

    }

}
