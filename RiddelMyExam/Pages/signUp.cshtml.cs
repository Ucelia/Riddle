using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace RiddelMyExam.Pages
{
    public class signUpModel : PageModel

    {
        public readonly IConfiguration Configuration;
        public signUpModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public user user = new user();
        
        public string errorMessage = "";
        public void OnGet()
        {
        }
        public void OnPost()
        {
            user.name = Request.Form["name"];
            user.email = Request.Form["email"];
            user.password = Request.Form["password"];
           // string constring = Configuration.GetConnectionString("con");

            if (user.name.Trim().Length == 0 || user.email.Trim().Length == 0 || user.password.Trim().Length == 0)
            {

                errorMessage = "please fill in";
            }
            else
            {
                string? constring = Configuration.GetConnectionString("con");
                try
                {

                    using (SqlConnection con = new SqlConnection(constring))
                    {
                       
                        string query = "insert into users values(@name,@email,@password,'user')";
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {

                            cmd.Parameters.AddWithValue("@name", user.name);
                            cmd.Parameters.AddWithValue("@email", user.email);
                            cmd.Parameters.AddWithValue("@password", user.password);
                            int rowAffected = cmd.ExecuteNonQuery();
                            if (rowAffected > 0)
                            {
                                errorMessage = "an account";
                                 Response.Redirect("/login");
                            }
                            else
                            {
                                errorMessage = "Failed to create an account";
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
    }
    public class user
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
    }
}
