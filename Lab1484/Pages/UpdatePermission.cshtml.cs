using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class UpdatePermissionModel : PageModel
    {
        //lists to store grants and users
        public List<Grant> GrantList { get; set; }
        public List<User> UserList { get; set; }

        public UpdatePermissionModel()
        {
            //Initialize the lists
            GrantList = new List<Grant>();
            UserList = new List<User>();
        }
        public void OnGet()
        {
            SqlDataReader grantReader = DBClass.GrantReader();
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = (int)grantReader["GrantID"],
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                    facultyName = grantReader["FacultyLead"].ToString(),
                    grantStatus = grantReader["grantStatus"].ToString()
                });
            }





        }
        public void OnPostSelectGrant(int GrantID)
        {
            SqlDataReader userReader = DBClass.Grant_UserReader(GrantID);
            while (userReader.Read())
            {
                int userID;
               
                    UserList.Add(new User
                    {
                        userID = (int)userReader["UserID"],
                        firstName = userReader["UserName"].ToString(),
                        lastName = userReader["LastName"].ToString(),
                        email = userReader["Email"].ToString(),
                        phone = userReader["Phone"].ToString(),

                    });
                }
                




            }
        }
    }

