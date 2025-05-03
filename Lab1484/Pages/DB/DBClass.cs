using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.InteropServices;
using Lab1484.Pages.DataClasses;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Lab1484.Pages.DB
{
    public class DBClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection Lab3DBConnection = new SqlConnection();

        //A second Connection Object at Data Field Level
        public static SqlConnection AUTHDBConnection = new SqlConnection();


        //Connection String - How to find and connect to DB - Uncomment when making local changes
        private static readonly String? Lab3DBConnString ="Server=LocalHost;Database=Lab3;Trusted_Connection=True";

        /*private static readonly String? Lab3DBConnString = "Server=seamless-solutions-server.database.windows.net,1433;" +
            "Database=Lab3;" +
            "User Id=capstoneadmin;" +
            "Password=Seamless123!@#;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;";*/


        // A second connection String - Uncomment when making local changes
        // For Hashed Passwords
        private static readonly String? AuthConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";

        /*private static readonly String? AuthConnString = "Server=seamless-solutions-server.database.windows.net,1433;" +
            "Database=AUTH;" +
            "User Id=capstoneadmin;" +
            "Password=Seamless123!@#;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;";*/

        //Connection Methods:

        //Basic Project Reader
        public static SqlDataReader ProjectReader(string? ProjectSearchQuery)
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;

            if (!string.IsNullOrEmpty(ProjectSearchQuery))
            {
                cmdProjectRead.CommandText = "Select Project.*, Concat(Users.firstName, ' ', Users.lastName) AS AdminName, Users.email AS AdminEmail " +
                    "from Project " +
                    "join Users ON Users.UserID = Project.ProjectAdminID " +
                    "where projectName LIKE @SearchQuery OR Concat(Users.firstName, ' ', Users.lastName) LIKE @SearchQuery OR Users.email LIKE @SearchQuery OR projectStatus LIKE @SearchQuery; ";
                cmdProjectRead.Parameters.AddWithValue("@SearchQuery", "%" + ProjectSearchQuery + "%");
            }
            else
                cmdProjectRead.CommandText = "Select Project.*, Concat(Users.firstName, ' ', Users.lastName) AS AdminName, Users.email AS AdminEmail " +
                "from Project " +
                "join Users ON Users.UserID = Project.ProjectAdminID; ";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }

        //Project Reader with ProjectID
        public static SqlDataReader SingleProjectReader(int ProjectID)
        {
            SqlCommand cmdSingleProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdSingleProjectRead.Connection = Lab3DBConnection;
            cmdSingleProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdSingleProjectRead.CommandText = "Select Project.*, Concat(Users.firstName, ' ', Users.lastName) AS AdminName " +
                "from Project " +
                "join Users ON Users.UserID = Project.ProjectAdminID" +
                " WHERE Project.ProjectID = @ProjectID; ";
            cmdSingleProjectRead.Parameters.AddWithValue("@ProjectID", ProjectID);
            cmdSingleProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdSingleProjectRead.ExecuteReader();

            return tempReader;
            cmdSingleProjectRead.Connection.Close();
        }



        //Update Project
        public static bool UpdateProject(Project p)
        {
            try
            {


                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }
                string sqlQuery = "UPDATE Project " +
                    "SET projectStatus = @ProjectStatus, dueDate = @DueDate, projectName = @ProjectName " +
                    "WHERE ProjectID = @ProjectID;";
                SqlCommand cmdProjectUpdate = new SqlCommand();
                cmdProjectUpdate.Connection = Lab3DBConnection;
                cmdProjectUpdate.Connection.ConnectionString = Lab3DBConnString;
                cmdProjectUpdate.CommandText = sqlQuery;
                cmdProjectUpdate.Parameters.AddWithValue("@ProjectStatus", p.ProjectStatus);
                cmdProjectUpdate.Parameters.AddWithValue("@DueDate", p.DateDue);
                cmdProjectUpdate.Parameters.AddWithValue("@ProjectName", p.ProjectName);
                cmdProjectUpdate.Parameters.AddWithValue("@ProjectID", p.ProjectID);
                cmdProjectUpdate.Connection.Open();
                cmdProjectUpdate.ExecuteNonQuery();

                return true;
            }

            catch 
            {
                return false;
            }

        }

        //Task Reader
        public static SqlDataReader TaskReader()
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdProjectRead.CommandText = "SELECT ProjTasks.*, Project.ProjectName, CONCAT(Users.FirstName, ' ', Users.LastName) AS 'UserName' " +
                "FROM ProjTasks " +
                "JOIN Project ON Project.ProjectID = ProjTasks.ProjectID " +
                "JOIN Users ON Users.UserID = ProjTasks.UserID " +
                "ORDER BY CASE WHEN PTStatus = 'Incomplete' THEN 1 WHEN PTStatus = 'Complete' THEN 2 END ASC, dueDate ASC;";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }

        //Task Reader with UserID
        public static SqlDataReader UserTaskReader(int UserID)
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdProjectRead.CommandText = "SELECT ProjTasks.*, Project.ProjectName, CONCAT(Users.FirstName, ' ', Users.LastName) AS 'UserName' " +
                "FROM ProjTasks " +
                "JOIN Project ON Project.ProjectID = ProjTasks.ProjectID " +
                "JOIN Users ON Users.UserID = ProjTasks.UserID " +
                "WHERE ProjTasks.UserID = @UserID " +
                "ORDER BY CASE WHEN PTStatus = 'Incomplete' THEN 1 WHEN PTStatus = 'Complete' THEN 2 END ASC, dueDate ASC;";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!
            cmdProjectRead.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }

        //Insert Task
        public static bool InsertTask(ProjTask t)
        {
            try
            {



                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }

                string sqlQuery = "INSERT INTO ProjTasks (ProjectID, UserID, taskDescription, dueDate) VALUES (@ProjectID, @UserID, @taskDescription, @dueDate);";

                SqlCommand cmdTaskInsert = new SqlCommand();
                cmdTaskInsert.Connection = Lab3DBConnection;
                cmdTaskInsert.Connection.ConnectionString = Lab3DBConnString;
                cmdTaskInsert.CommandText = sqlQuery;
                cmdTaskInsert.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmdTaskInsert.Parameters.AddWithValue("@UserID", t.UserID);
                cmdTaskInsert.Parameters.AddWithValue("@taskDescription", t.taskDescription);
                cmdTaskInsert.Parameters.AddWithValue("@dueDate", t.dueDate);
                cmdTaskInsert.Connection.Open();
                cmdTaskInsert.ExecuteNonQuery();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void TaskComplete(int TaskID)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = "UPDATE ProjTasks " +
                "SET PTStatus = 'Complete' " +
                "WHERE TaskID = @TaskID;";
            SqlCommand cmdTaskUpdate = new SqlCommand();
            cmdTaskUpdate.Connection = Lab3DBConnection;
            cmdTaskUpdate.Connection.ConnectionString = Lab3DBConnString;
            cmdTaskUpdate.CommandText = sqlQuery;
            cmdTaskUpdate.Parameters.AddWithValue("@TaskID", TaskID);
            cmdTaskUpdate.Connection.Open();
            cmdTaskUpdate.ExecuteNonQuery();
        }

        public static SqlDataReader GrantTaskReader()
        {
            SqlCommand cmdGrantTaskRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdGrantTaskRead.Connection = Lab3DBConnection;
            cmdGrantTaskRead.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantTaskRead.CommandText = "SELECT GrantTasks.*, Grants.grantName, CONCAT(Users.FirstName, ' ', Users.LastName) AS 'UserName' " +
                "FROM GrantTasks " +
                "JOIN Grants ON Grants.GrantID = GrantTasks.GrantID " +
                "JOIN Users ON Users.UserID = GrantTasks.UserID " +
                "ORDER BY CASE WHEN GTStatus = 'Incomplete' THEN 1 WHEN GTStatus = 'Complete' THEN 2 END ASC, dueDate ASC;";
            cmdGrantTaskRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdGrantTaskRead.ExecuteReader();

            return tempReader;
            cmdGrantTaskRead.Connection.Close();
        }

        //Grant Task Reader with UserID
        public static SqlDataReader UserGrantTaskReader(int UserID)
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdProjectRead.CommandText = "SELECT GrantTasks.*, Grants.grantName, CONCAT(Users.FirstName, ' ', Users.LastName) AS 'UserName' " +
                "FROM GrantTasks " +
                "JOIN Grants ON Grants.GrantID = GrantTasks.GrantID " +
                "JOIN Users ON Users.UserID = GrantTasks.UserID " +
                "WHERE GrantTasks.UserID = @UserID " +
                "ORDER BY CASE WHEN GTStatus = 'Incomplete' THEN 1 WHEN GTStatus = 'Complete' THEN 2 END ASC, dueDate ASC;";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!
            cmdProjectRead.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }

        //Read GrantTask for specific Grant
        public static SqlDataReader SpecGrantTaskReader(int GrantID)
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdProjectRead.CommandText = "SELECT GrantTasks.*, Grants.grantName, CONCAT(Users.FirstName, ' ', Users.LastName) AS 'UserName' " +
                "FROM GrantTasks " +
                "JOIN Grants ON Grants.GrantID = GrantTasks.GrantID " +
                "JOIN Users ON Users.UserID = GrantTasks.UserID " +
                "WHERE GrantTasks.GrantID = @GrantID " +
                "ORDER BY CASE WHEN GTStatus = 'Incomplete' THEN 1 WHEN GTStatus = 'Complete' THEN 2 END ASC, dueDate ASC;";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!
            cmdProjectRead.Parameters.AddWithValue("@GrantID", GrantID);
            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }

        //Read one grant
        public static SqlDataReader SpecGrantReader(int GrantID)
        {
            SqlCommand cmdSpecGrantRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdSpecGrantRead.Connection = Lab3DBConnection;
            cmdSpecGrantRead.Connection.ConnectionString = Lab3DBConnString;
            cmdSpecGrantRead.CommandText = "SELECT Grants.*, CONCAT(Users.firstName, ' ', Users.lastName) AS 'UserName' FROM Grants " +
                "JOIN Users ON Grants.FacultyLeadID = Users.UserID " +
                "WHERE Grants.GrantID = @GrantID;";
            cmdSpecGrantRead.Connection.Open(); // Open connection here, close in Model!
            cmdSpecGrantRead.Parameters.AddWithValue("@GrantID", GrantID);
            SqlDataReader tempReader = cmdSpecGrantRead.ExecuteReader();

            return tempReader;
            cmdSpecGrantRead.Connection.Close();
        }

        //Insert  GrantTask
        public static bool InsertGrantTask(GrantTask g)
        {
            try
            {


                
                    if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                    {
                        Lab3DBConnection.Close();
                    }

                    string sqlQuery = "INSERT INTO GrantTasks (GrantID, UserID, taskDescription, dueDate) VALUES (@GrantID, @UserID, @taskDescription, @dueDate);";

                    SqlCommand cmdGrantTaskInsert = new SqlCommand();
                    cmdGrantTaskInsert.Connection = Lab3DBConnection;
                    cmdGrantTaskInsert.Connection.ConnectionString = Lab3DBConnString;
                    cmdGrantTaskInsert.CommandText = sqlQuery;
                    cmdGrantTaskInsert.Parameters.AddWithValue("@GrantID", g.GrantID);
                    cmdGrantTaskInsert.Parameters.AddWithValue("@UserID", g.UserID);
                    cmdGrantTaskInsert.Parameters.AddWithValue("@taskDescription", g.taskDescription);
                    cmdGrantTaskInsert.Parameters.AddWithValue("@dueDate", g.dueDate);
                    cmdGrantTaskInsert.Connection.Open();
                    cmdGrantTaskInsert.ExecuteNonQuery();
                

                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        public static void GrantTaskComplete(int GTaskID)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = "UPDATE GrantTasks " +
                "SET GTStatus = 'Complete' " +
                "WHERE GTaskID = @GTaskID;";
            SqlCommand cmdGrantTaskUpdate = new SqlCommand();
            cmdGrantTaskUpdate.Connection = Lab3DBConnection;
            cmdGrantTaskUpdate.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantTaskUpdate.CommandText = sqlQuery;
            cmdGrantTaskUpdate.Parameters.AddWithValue("@GTaskID", GTaskID);
            cmdGrantTaskUpdate.Connection.Open();
            cmdGrantTaskUpdate.ExecuteNonQuery();
        }


        public static SqlDataReader GrantReader(string? SearchQuery)//reads grant table in sql
        {
            SqlCommand cmdGrantRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            cmdGrantRead.Connection = Lab3DBConnection;
            cmdGrantRead.Connection.ConnectionString = Lab3DBConnString;


            if (!string.IsNullOrEmpty(SearchQuery))
            {
                cmdGrantRead.CommandText = "Select Grants.*, Concat(Users.firstName, ' ', Users.lastName) AS FacultyLead, Users.email AS FacultyLeadEmail " +
                    "from Grants " +
                    "join Users ON Users.UserID = Grants.FacultyLeadID " +
                    "where grantName LIKE @SearchQuery OR Concat(Users.firstName, ' ', Users.lastName) LIKE @SearchQuery OR Users.email LIKE @SearchQuery OR amount LIKE @SearchQuery OR dueDate LIKE @SearchQuery OR grantStatus LIKE @SearchQuery OR businessName LIKE @SearchQuery OR category LIKE @SearchQuery " +
                    "ORDER BY CASE WHEN grantStatus = 'Active' THEN 1 WHEN grantStatus = 'Funded' THEN 2 WHEN grantStatus = 'Potential' THEN 3 WHEN grantStatus = 'Rejected' THEN 4 WHEN grantStatus = 'Archived' THEN 5 END ASC, Grants.dueDate ASC;";

                cmdGrantRead.Parameters.AddWithValue("@SearchQuery", "%" + SearchQuery + "%");

            }

            else
            {
                cmdGrantRead.CommandText = "Select Grants.*, Concat(Users.firstName, ' ', Users.lastName) AS FacultyLead, Users.email AS FacultyLeadEmail " +
                    "from Grants " +
                    "join Users ON Users.UserID = Grants.FacultyLeadID " +
                    "ORDER BY CASE WHEN grantStatus = 'Active' THEN 1 WHEN grantStatus = 'Funded' THEN 2 WHEN grantStatus = 'Potential' THEN 3 WHEN grantStatus = 'Rejected' THEN 4 WHEN grantStatus = 'Archived' THEN 5 END ASC, Grants.dueDate ASC; ";
            }

            cmdGrantRead.Connection.Open(); // Open connection here, close in Model!
            SqlDataReader tempReader = cmdGrantRead.ExecuteReader();

            return tempReader;
        }




        public static SqlDataReader AdminReader()//reads admin table
        {
            SqlCommand cmdAdminRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdAdminRead.Connection = Lab3DBConnection;
            cmdAdminRead.Connection.ConnectionString = Lab3DBConnString;
            cmdAdminRead.CommandText = "SELECT Users.userID, Users.firstName, Users.lastName " +
                "FROM Users " +
                "WHERE Users.userType=0;";
            cmdAdminRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAdminRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        public static SqlDataReader EmployeeReader()//reads employee table
        {
            SqlCommand cmdEmployeeRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdEmployeeRead.Connection = Lab3DBConnection;
            cmdEmployeeRead.Connection.ConnectionString = Lab3DBConnString;
            cmdEmployeeRead.CommandText = "SELECT Users.userID, Users.firstName, Users.lastName " +
                "FROM Users " +
                "WHERE Users.userType=2;";
            cmdEmployeeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdEmployeeRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        public static SqlDataReader AllUsersReader() // Reads all users
        {
            SqlCommand cmdAllUsersRead = new SqlCommand();

            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            cmdAllUsersRead.Connection = Lab3DBConnection;
            cmdAllUsersRead.Connection.ConnectionString = Lab3DBConnString;

            // Removed WHERE clause to include ALL users
            cmdAllUsersRead.CommandText = @"
        SELECT userID, firstName, lastName
        FROM Users
        ORDER BY firstName, lastName;";

            cmdAllUsersRead.Connection.Open(); // Open connection here, close it in the model

            SqlDataReader tempReader = cmdAllUsersRead.ExecuteReader();

            return tempReader;
        }


        public static SqlDataReader FacultyReader()//reads employee table
        {
            SqlCommand cmdEmployeeRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdEmployeeRead.Connection = Lab3DBConnection;
            cmdEmployeeRead.Connection.ConnectionString = Lab3DBConnString;
            cmdEmployeeRead.CommandText = "SELECT Users.userID, Users.firstName, Users.lastName " +
                "FROM Users " +
                "WHERE Users.userType=1;";
            cmdEmployeeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdEmployeeRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }
        public static SqlDataReader BusinessPartnerReader()//reads employee table
        {
            SqlCommand cmdPartnerRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdPartnerRead.Connection = Lab3DBConnection;
            cmdPartnerRead.Connection.ConnectionString = Lab3DBConnString;
            cmdPartnerRead.CommandText = "SELECT BusinessPartner.BusinessPartnerID, BusinessPartner.firstName," +
                " BusinessPartner.lastName " +
                "FROM BusinessPartner;";
            cmdPartnerRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPartnerRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        public static int InsertProject(Project p)//inserts new project into DB
        {

            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = "INSERT INTO Project " +
                "(ProjectAdminID, projectStatus, dateCreated, dateCompleted, dueDate, projectName)" +
                " VALUES (@ProjectAdminID, @ProjectStatus, @DateCreated, @CompletionDate, @DueDate, @ProjectName);" +
                "SELECT Scope_Identity();";


            SqlCommand cmdProjectInsert = new SqlCommand();
            cmdProjectInsert.Connection = Lab3DBConnection;
            cmdProjectInsert.Connection.ConnectionString = Lab3DBConnString;
            cmdProjectInsert.CommandText = sqlQuery;
            cmdProjectInsert.Parameters.AddWithValue("@ProjectAdminID", p.ProjectAdminID);
            cmdProjectInsert.Parameters.AddWithValue("@ProjectStatus", p.ProjectStatus);
            cmdProjectInsert.Parameters.AddWithValue("@DateCreated", p.DateCreated);
            cmdProjectInsert.Parameters.AddWithValue("@CompletionDate", p.DateCompleted);
            cmdProjectInsert.Parameters.AddWithValue("@DueDate", p.DateDue);
            cmdProjectInsert.Parameters.AddWithValue("@ProjectName", p.ProjectName);

            cmdProjectInsert.Connection.Open();

            //Get ProjectID so that EmployeeProject can be updated
            object result = cmdProjectInsert.ExecuteScalar();
            int newProjectID = Convert.ToInt32(result);

            return newProjectID;
        }

        public static int GetLastGrantID()
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = "SELECT MAX(GrantID) FROM Grants;";
            SqlCommand cmdGrantRead = new SqlCommand();
            cmdGrantRead.Connection = Lab3DBConnection;
            cmdGrantRead.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantRead.CommandText = sqlQuery;
            cmdGrantRead.Connection.Open();
            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdGrantRead.ExecuteScalar();
            return rowCount;
        }

        public static void InsertEmployeeProject(int ProjectID, int EmployeeID)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string sqlQuery = "INSERT INTO EmployeeProject (ProjectID, EmployeeID) VALUES (@ProjectID, @EmployeeID);";

            SqlCommand cmdEmployeeProjectInsert = new SqlCommand();
            cmdEmployeeProjectInsert.Connection = Lab3DBConnection;
            cmdEmployeeProjectInsert.Connection.ConnectionString = Lab3DBConnString;
            cmdEmployeeProjectInsert.CommandText = sqlQuery;
            cmdEmployeeProjectInsert.Parameters.AddWithValue("@ProjectID", ProjectID);
            cmdEmployeeProjectInsert.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmdEmployeeProjectInsert.Connection.Open();
            cmdEmployeeProjectInsert.ExecuteNonQuery();
        }

        //Inserts User into DB
        public static void InsertUser(User p)//inserts user into sql
        {

            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = @"
             INSERT INTO Users (userType, firstName, lastName, email, phoneNumber)
             VALUES (@UserType, @firstName, @lastName, @email, @phoneNumber);

             DECLARE @UserIDLogin INT;
             SET @UserIDLogin = SCOPE_IDENTITY();";

            SqlCommand cmdProjectRead = new SqlCommand();
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString =
            Lab3DBConnString;
            cmdProjectRead.CommandText = sqlQuery;

            cmdProjectRead.Parameters.AddWithValue("@UserType", p.UserType);
            cmdProjectRead.Parameters.AddWithValue("@firstName", p.firstName);
            cmdProjectRead.Parameters.AddWithValue("@lastName", p.lastName);
            cmdProjectRead.Parameters.AddWithValue("@email", p.email);
            cmdProjectRead.Parameters.AddWithValue("@phoneNumber", p.phone);
            cmdProjectRead.Connection.Open();
            cmdProjectRead.ExecuteNonQuery();

        }






        public static bool InsertGrant(Grant g)
        {
            try
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }

                string sqlQuery = @"INSERT INTO Grants (FacultyLeadID, BusinessPartnerID, businessName,
                    category, dueDate, grantStatus, amount, grantName)
                    VALUES (@FacultyLeadID, @BusinessPartnerID, @BusinessName,
                    @Category, @DueDate, @GrantStatus, @Amount, @GrantName);";

                using (SqlCommand cmdGrantInsert = new SqlCommand(sqlQuery, Lab3DBConnection))
                {
                    cmdGrantInsert.Connection.ConnectionString = Lab3DBConnString;

                    cmdGrantInsert.Parameters.AddWithValue("@FacultyLeadID", g.FacultyLeadID);
                    cmdGrantInsert.Parameters.AddWithValue("@BusinessPartnerID", g.BusinessPartnerID);
                    cmdGrantInsert.Parameters.AddWithValue("@BusinessName", g.businessName);
                    cmdGrantInsert.Parameters.AddWithValue("@Category", g.category);
                    cmdGrantInsert.Parameters.AddWithValue("@DueDate", g.dueDate);
                    cmdGrantInsert.Parameters.AddWithValue("@GrantStatus", g.grantStatus);
                    cmdGrantInsert.Parameters.AddWithValue("@Amount", g.amount);
                    cmdGrantInsert.Parameters.AddWithValue("@GrantName", g.grantName);

                    cmdGrantInsert.Connection.Open();
                    cmdGrantInsert.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //Update Grant
        public static bool UpdateGrant(Grant g)
        {
            try
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }

                string sqlQuery = @"
            UPDATE Grants
            SET 
                FacultyLeadID = @FacultyLeadID,
                BusinessPartnerID = @BusinessPartnerID,
                businessName = @businessName,
                category = @category,
                dueDate = @dueDate,
                grantStatus = @grantStatus,
                amount = @amount,
                grantName = @grantName
            WHERE GrantID = @GrantID;";

                using (SqlCommand cmdGrantUpdate = new SqlCommand(sqlQuery, Lab3DBConnection))
                {
                    cmdGrantUpdate.Connection.ConnectionString = Lab3DBConnString;

                    cmdGrantUpdate.Parameters.AddWithValue("@FacultyLeadID", g.FacultyLeadID);
                    cmdGrantUpdate.Parameters.AddWithValue("@BusinessPartnerID", g.BusinessPartnerID);
                    cmdGrantUpdate.Parameters.AddWithValue("@businessName", g.businessName);
                    cmdGrantUpdate.Parameters.AddWithValue("@category", g.category);
                    cmdGrantUpdate.Parameters.AddWithValue("@dueDate", g.dueDate);
                    cmdGrantUpdate.Parameters.AddWithValue("@grantStatus", g.grantStatus);
                    cmdGrantUpdate.Parameters.AddWithValue("@amount", g.amount);
                    cmdGrantUpdate.Parameters.AddWithValue("@grantName", g.grantName);
                    cmdGrantUpdate.Parameters.AddWithValue("@GrantID", g.GrantID);

                    cmdGrantUpdate.Connection.Open();
                    cmdGrantUpdate.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }





        public static SqlDataReader EmployeeProjectReader()
        {
            SqlCommand cmdEmployeeProjectRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdEmployeeProjectRead.Connection = Lab3DBConnection;
            cmdEmployeeProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdEmployeeProjectRead.CommandText = "SELECT EmployeeProject.* " +
                "FROM EmployeeProject;";
            cmdEmployeeProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdEmployeeProjectRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        //Login methods:
        public static int LoginQuery(string loginQuery)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            // This method expects to receive an SQL SELECT
            // query that uses the COUNT command.

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab3DBConnection;
            cmdLogin.Connection.ConnectionString = Lab3DBConnString;
            cmdLogin.CommandText = loginQuery;
            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdLogin.ExecuteScalar();

            return rowCount;
        }


        public static int SecureLogin(string Username, string Password)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string loginQuery =
                "SELECT COUNT(*) FROM Credentials where Username = @Username and Password = @Password";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab3DBConnection;
            cmdLogin.Connection.ConnectionString = Lab3DBConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.Parameters.AddWithValue("@Password", Password);

            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdLogin.ExecuteScalar();

            return rowCount;
        }
        //new
        public static List<MessagesModel> GetConversationSummaries(string currentUser)
        {
            var messages = new List<MessagesModel>();

            string query = @"
        WITH UserConversations AS (
            SELECT 
                CASE 
                    WHEN Sender = @currentUser THEN Receiver
                    ELSE Sender
                END AS OtherUser,
                MAX(Timestamp) AS LatestTime
            FROM Messages
            WHERE Sender = @currentUser OR Receiver = @currentUser
            GROUP BY CASE 
                        WHEN Sender = @currentUser THEN Receiver
                        ELSE Sender
                     END
        )
        SELECT 
            uc.OtherUser,
            m.Content AS LastMessage,
            m.Timestamp AS LatestTime
        FROM UserConversations uc
        JOIN Messages m ON 
            ((m.Sender = @currentUser AND m.Receiver = uc.OtherUser) OR
             (m.Receiver = @currentUser AND m.Sender = uc.OtherUser))
            AND m.Timestamp = uc.LatestTime
        ORDER BY LatestTime DESC;
    ";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@currentUser", currentUser);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    messages.Add(new MessagesModel
                    {
                        Receiver = reader["OtherUser"].ToString(),
                        Content = reader["LastMessage"].ToString(),
                        SentDate = (DateTime)reader["LatestTime"]
                    });
                }
            }

            return messages;
        }

        public static void SendMessage(string sender, string receiver, string content, string? fileName, string? filePath)
        {
            string query = @"
        INSERT INTO Messages (Sender, Receiver, Content, Timestamp, IsRead, AttachmentFileName, AttachmentFilePath)
        VALUES (@Sender, @Receiver, @Content, GETDATE(), 0, @AttachmentFileName, @AttachmentFilePath)";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Sender", sender);
                cmd.Parameters.AddWithValue("@Receiver", receiver);
                cmd.Parameters.AddWithValue("@Content", content);
                cmd.Parameters.AddWithValue("@AttachmentFileName", (object?)fileName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttachmentFilePath", (object?)filePath ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }



        public static List<MessagesModel> GetAllMessagesForUser(string username)
        {
            List<MessagesModel> messages = new List<MessagesModel>();

            string query = @"
        SELECT MessageId, Sender, Receiver, Content, Timestamp AS SentDate, IsRead
        FROM Messages
        WHERE Sender = @Username OR Receiver = @Username
        ORDER BY Timestamp";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    messages.Add(new MessagesModel
                    {
                        MessageId = (int)reader["MessageId"],
                        Sender = reader["Sender"].ToString(),
                        Receiver = reader["Receiver"].ToString(),
                        Content = reader["Content"].ToString(),
                        SentDate = (DateTime)reader["SentDate"],
                        IsRead = Convert.ToInt32(reader["IsRead"])
                    });
                }
            }

            return messages;
        }

        public static List<MessagesModel> GetConversationBetween(string user1, string user2)
        {
            List<MessagesModel> messages = new List<MessagesModel>();

            string query = @"
        SELECT MessageId, Sender, Receiver, Content, Timestamp AS SentDate, IsRead
        FROM Messages
        WHERE (Sender = @User1 AND Receiver = @User2) OR (Sender = @User2 AND Receiver = @User1)
        ORDER BY Timestamp ASC";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@User1", user1);
                cmd.Parameters.AddWithValue("@User2", user2);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    messages.Add(new MessagesModel
                    {
                        MessageId = (int)reader["MessageId"],
                        Sender = reader["Sender"].ToString(),
                        Receiver = reader["Receiver"].ToString(),
                        Content = reader["Content"].ToString(),
                        SentDate = (DateTime)reader["SentDate"],
                        IsRead = Convert.ToInt32(reader["IsRead"])
                    });
                }
            }

            return messages;
        }

        //Messages methods:
        public static List<MessagesModel> GetReceivedMessages(string receiver)
        {
            List<MessagesModel> messages = new List<MessagesModel>();

            string query = @"
        SELECT MessageId, Sender, Receiver, Content, Timestamp AS SentDate, IsRead
        FROM Messages
        WHERE Receiver = @Receiver
        ORDER BY Timestamp DESC";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Receiver", receiver);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MessagesModel message = new MessagesModel
                            {
                                MessageId = (int)reader["MessageId"],
                                Sender = reader["Sender"].ToString(),
                                Receiver = reader["Receiver"].ToString(),
                                Content = reader["Content"].ToString(),
                                SentDate = (DateTime)reader["SentDate"],
                                //IsRead = (int)reader["IsRead"]
                            };
                            messages.Add(message);
                        }
                    }
                }
            }
            return messages;
        }

        public static void MarkMessageAsRead(int messageId)
        {
            string query = "UPDATE Messages SET IsRead = 1 WHERE MessageId = @MessageId";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MessageId", messageId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }



        // Method to get the count of unread messages for the current user
        public static int GetUnreadMessagesCount(string receiver)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            // Query to count unread messages for the given receiver
            string query = "SELECT COUNT(*) FROM Messages WHERE Receiver = @Receiver AND IsRead = 0";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Lab3DBConnection;
            cmd.Connection.ConnectionString = Lab3DBConnString;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@Receiver", receiver);

            cmd.Connection.Open();

            int unreadCount = (int)cmd.ExecuteScalar();

            cmd.Connection.Close(); // Close connection after execution

            return unreadCount;
        }


        // Method to get all messages for a specific user (Receiver)
        public static List<MessagesModel> GetUserMessages(string receiver)
        {
            List<MessagesModel> messages = new List<MessagesModel>();
            string query = "SELECT * FROM Messages WHERE Receiver = @Receiver ORDER BY TimeStamp DESC";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Receiver", receiver);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Populate the messages list with instances of MessageData
                    MessagesModel message = new MessagesModel
                    {
                        MessageId = (int)reader["MessageId"],
                        Sender = reader["Sender"].ToString(),
                        Receiver = reader["Receiver"].ToString(),
                        Content = reader["Content"].ToString(),
                        SentDate = (DateTime)reader["Timestamp"],

                        // IsRead = (int)reader["IsRead"]

                        //IsRead = (int)reader["IsRead"]
                    };
                    messages.Add(message); // Add the message instance to the list
                }
            }
            return messages; // Return the list of messages
        }

        public static List<MessagesModel> GetUserSentMessages(string sender)
        {
            List<MessagesModel> messages = new List<MessagesModel>();

            string query = @"
        SELECT MessageId, Sender, Receiver, Content, Timestamp AS SentDate, IsRead 
        FROM Messages 
        WHERE Sender = @Sender 
        ORDER BY Timestamp DESC"; // Order by the real database Timestamp

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Sender", sender);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MessagesModel message = new MessagesModel
                            {
                                MessageId = reader["MessageId"] != DBNull.Value ? (int)reader["MessageId"] : 0,
                                Sender = reader["Sender"]?.ToString(),
                                Receiver = reader["Receiver"]?.ToString(),
                                Content = reader["Content"]?.ToString(),
                                SentDate = reader["SentDate"] != DBNull.Value ? (DateTime)reader["SentDate"] : DateTime.MinValue,
                                IsRead = reader["IsRead"] != DBNull.Value ? Convert.ToInt32(reader["IsRead"]) : 0
                            };
                            messages.Add(message);
                        }
                    }
                }
            }
            return messages;
        }



        public static SqlDataReader MessageReader()
        {
            SqlCommand cmdMessageRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdMessageRead.Connection = Lab3DBConnection;
            cmdMessageRead.Connection.ConnectionString = Lab3DBConnString;
            cmdMessageRead.CommandText = "Select Messages.* FROM Messages;";
            cmdMessageRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdMessageRead.ExecuteReader();

            return tempReader;
            cmdMessageRead.Connection.Close();
        }
        //end
        

        public static SqlDataReader CredentialsReader()
        {

            SqlCommand cmdCredentialsRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdCredentialsRead.Connection = Lab3DBConnection;
            cmdCredentialsRead.Connection.ConnectionString = AuthConnString;
            cmdCredentialsRead.CommandText = "Select HashedCredentials.Username FROM HashedCredentials;";
            cmdCredentialsRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdCredentialsRead.ExecuteReader();

            return tempReader;
            cmdCredentialsRead.Connection.Close();
        }

        //Inserts new message into DB
        public static void InsertMessage(Message m)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = "INSERT INTO Messages (Sender, Receiver, Content)" +
                " VALUES (@Sender, @Receiver, @Content);";

            Lab3DBConnection.ConnectionString = Lab3DBConnString;

            using (SqlCommand cmdMessageInsert = new SqlCommand(sqlQuery, Lab3DBConnection))
            {
                cmdMessageInsert.Parameters.AddWithValue("@Sender", m.Sender);
                cmdMessageInsert.Parameters.AddWithValue("@Receiver", m.Receiver);
                cmdMessageInsert.Parameters.AddWithValue("@Content", m.Content);

                cmdMessageInsert.Connection.Open();
                cmdMessageInsert.ExecuteNonQuery();
            }
        }



        //Methods for Creating a User and Login with Password Hashing

        public static bool HashedParameterLogin(string Username, string Password, HttpContext httpcontext)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string loginQuery = "sp_Lab3Login";
            //"SELECT Password FROM HashedCredentials WHERE Username = @Username";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab3DBConnection;
            cmdLogin.Connection.ConnectionString = AuthConnString;
            cmdLogin.CommandText = loginQuery;

            cmdLogin.CommandType = CommandType.StoredProcedure;
            cmdLogin.Parameters.AddWithValue("@Username", Username);

            cmdLogin.Connection.Open();

            SqlDataReader hashReader = cmdLogin.ExecuteReader();


            if (hashReader.Read())
            {
                string correctHash = hashReader["Password"].ToString();
                int userID = Convert.ToInt32(hashReader["UserID"]);

                if (PasswordHash.ValidatePassword(Password, correctHash))
                {
                    httpcontext.Session.SetString("userID", userID.ToString());//this sets the session userID data to the userID of the authenticated user
                    return true;
                }
            }

            return false;
        }


        public static bool CreateHashedUser(User p)
        {
            try
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }

                int userID;

                using (SqlCommand cmdUserInsert = new SqlCommand())
                {
                    cmdUserInsert.Connection = Lab3DBConnection;
                    cmdUserInsert.CommandText = @"
                INSERT INTO Users (userType, firstName, lastName, email, phoneNumber)
                VALUES (@UserType, @firstName, @lastName, @email, @phoneNumber);
                SELECT SCOPE_IDENTITY();";

                    cmdUserInsert.Parameters.AddWithValue("@UserType", p.UserType);
                    cmdUserInsert.Parameters.AddWithValue("@firstName", p.firstName);
                    cmdUserInsert.Parameters.AddWithValue("@lastName", p.lastName);
                    cmdUserInsert.Parameters.AddWithValue("@email", p.email);
                    cmdUserInsert.Parameters.AddWithValue("@phoneNumber", p.phone);

                    cmdUserInsert.Connection.Open();
                    userID = Convert.ToInt32(cmdUserInsert.ExecuteScalar());
                    cmdUserInsert.Connection.Close();
                }

                using (SqlConnection authConn = new SqlConnection(AuthConnString))
                using (SqlCommand cmdNewHashed = new SqlCommand())
                {
                    cmdNewHashed.Connection = authConn;
                    cmdNewHashed.CommandText = @"
                INSERT INTO HashedCredentials (UserID, Username, Password)
                VALUES (@UserID, @Username, @Password);";

                    cmdNewHashed.Parameters.AddWithValue("@UserID", userID);
                    cmdNewHashed.Parameters.AddWithValue("@Username", p.username);
                    cmdNewHashed.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(p.password));

                    authConn.Open();
                    cmdNewHashed.ExecuteNonQuery();
                    authConn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public static bool UpdateHashedUser(UserUpdate p)
        {
            try
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }

                string userUpdatedQuery = @"
         UPDATE Users
         SET userType = @UserType, firstName = @FirstName, lastName = @LastName, email = @Email, phoneNumber = @Phone
         WHERE UserID = @UserID;";

                SqlCommand cmdUserUpdate = new SqlCommand();
                cmdUserUpdate.Connection = Lab3DBConnection;
                cmdUserUpdate.CommandText = userUpdatedQuery;

                cmdUserUpdate.Parameters.AddWithValue("@UserID", p.UserID);
                cmdUserUpdate.Parameters.AddWithValue("@UserType", p.UserType);
                cmdUserUpdate.Parameters.AddWithValue("@FirstName", p.FirstName);
                cmdUserUpdate.Parameters.AddWithValue("@LastName", p.LastName);
                cmdUserUpdate.Parameters.AddWithValue("@Email", p.Email);
                cmdUserUpdate.Parameters.AddWithValue("@Phone", p.Phone);

                cmdUserUpdate.Connection.Open();
                cmdUserUpdate.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }




        public static bool UpdateHashedPassword(string userID, string newPassword)
        {
            if (Lab3DBConnection.State == ConnectionState.Open)
                Lab3DBConnection.Close();

            string updatePasswordQuery = @"
        UPDATE HashedCredentials
        SET Password = @NewHashedPassword
        WHERE UserID = @userID;";

            using (SqlCommand cmdUpdatePassword = new SqlCommand(updatePasswordQuery, new SqlConnection(AuthConnString)))
            {
                cmdUpdatePassword.Parameters.AddWithValue("@userID", userID);
                cmdUpdatePassword.Parameters.AddWithValue("@NewHashedPassword", PasswordHash.HashPassword(newPassword));

                cmdUpdatePassword.Connection.Open();
                int rowsAffected = cmdUpdatePassword.ExecuteNonQuery();
                cmdUpdatePassword.Connection.Close();

                return rowsAffected > 0; //true if password was updated
            }
        }






        //Get Notes for Proj
        public static List<Note> GetProjNotes(int ProjectID)
        {
            List<Note> Notes = new List<Note>();

            string sqlQuery = "SELECT * FROM Notes WHERE Notes.ProjectID = @ProjectID;";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@ProjectID", ProjectID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Populate Notes list
                    Note newNote = new Note
                    {
                        NoteID = (int)reader["NoteID"],
                        NoteBody = (string)reader["noteBody"],
                        ProjectID = (int)reader["ProjectID"]
                    };
                    Notes.Add(newNote); // Add the message instance to the list
                }
            }

            return Notes;
        }

        public static bool InsertNote(Note n)
        {
            try
            {


                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }
                string sqlQuery = "INSERT INTO Notes (ProjectID, noteBody) VALUES (@ProjectID, @noteBody);";


                SqlCommand cmdNoteInsert = new SqlCommand();
                cmdNoteInsert.Connection = Lab3DBConnection;
                cmdNoteInsert.Connection.ConnectionString = Lab3DBConnString;
                cmdNoteInsert.CommandText = sqlQuery;
                cmdNoteInsert.Parameters.AddWithValue("@ProjectID", n.ProjectID);
                cmdNoteInsert.Parameters.AddWithValue("@noteBody", n.NoteBody);

                cmdNoteInsert.Connection.Open();
                cmdNoteInsert.ExecuteNonQuery();

                return true;
            }
            catch 
            { 
                return false; 
            }
        }
        public static int checkUserType(HttpContext httpContext)//Calls httpContext to pull UserID 
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string findUserType = "getUserType"; // Stored procedure to find userType given userID from login method

            using (SqlConnection Lab3DBConnection = new SqlConnection(Lab3DBConnString))
            using (SqlCommand cmdCheckUser = new SqlCommand())
            {
                cmdCheckUser.Connection = Lab3DBConnection;

                cmdCheckUser.CommandText = findUserType;
                cmdCheckUser.CommandType = CommandType.StoredProcedure;

                string userID = httpContext.Session.GetString("userID");


                cmdCheckUser.Parameters.AddWithValue("@UserID", Convert.ToInt32(userID));

                cmdCheckUser.Connection.Open();

                using (SqlDataReader userTypeReader = cmdCheckUser.ExecuteReader())
                {
                    if (userTypeReader.Read())
                    {
                        return Convert.ToInt32(userTypeReader["UserType"]);
                    }
                    else
                    {
                        throw new Exception("UserType not found for the given userID.");
                    }
                }
            }

        }
        public static void updatePermission(grant_user g)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string checkPermission = "checkPermissionRecord";
            string updatePermission = "updatePermissionRecord";
            string insertPermission = "insertPermissionRecord";

            //Need if statement to check if theres already a record for the user and grant in the table
            //If there is, update the record
            //If there isn't, insert a new record THEN Update.. 
            SqlCommand cmdCheckUserPermission = new SqlCommand();
            cmdCheckUserPermission.Connection = Lab3DBConnection;

            cmdCheckUserPermission.CommandText = checkPermission;
            cmdCheckUserPermission.CommandType = CommandType.StoredProcedure;

            cmdCheckUserPermission.Parameters.AddWithValue("@UserID", Convert.ToInt32(g.userID));
            cmdCheckUserPermission.Parameters.AddWithValue("@GrantID", Convert.ToInt32(g.grantID));



            Lab3DBConnection.Open();



            //If there is a record, update it
            if (cmdCheckUserPermission.ExecuteScalar() != null)
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }
                SqlCommand cmdUpdateUserPermission = new SqlCommand();
                cmdUpdateUserPermission.Connection = Lab3DBConnection;
                cmdUpdateUserPermission.CommandText = updatePermission;
                cmdUpdateUserPermission.CommandType = CommandType.StoredProcedure;
                cmdUpdateUserPermission.Parameters.AddWithValue("@UserID", Convert.ToInt32(g.userID));
                cmdUpdateUserPermission.Parameters.AddWithValue("@GrantID", Convert.ToInt32(g.grantID));
                cmdUpdateUserPermission.Parameters.AddWithValue("@ViewPermission", Convert.ToInt32(g.viewPermission));
                cmdUpdateUserPermission.Parameters.AddWithValue("@EditPermission", Convert.ToInt32(g.editPermission));
                cmdUpdateUserPermission.Parameters.AddWithValue("@SensitivePermission", Convert.ToInt32(g.sensitiveInfoPermission));

                Lab3DBConnection.Open();
                cmdUpdateUserPermission.ExecuteNonQuery();



            }
            else //If there isn't a record, insert a new one then allow up
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }
                SqlCommand cmdInsertUserPermission = new SqlCommand();
                cmdInsertUserPermission.Connection = Lab3DBConnection;
                cmdInsertUserPermission.CommandText = insertPermission;
                cmdInsertUserPermission.CommandType = CommandType.StoredProcedure;
                cmdInsertUserPermission.Parameters.AddWithValue("@UserID", Convert.ToInt32(g.userID));
                cmdInsertUserPermission.Parameters.AddWithValue("@GrantID", Convert.ToInt32(g.grantID));
                cmdInsertUserPermission.Parameters.AddWithValue("@ViewPermission", Convert.ToInt32(g.viewPermission));
                cmdInsertUserPermission.Parameters.AddWithValue("@EditPermission", Convert.ToInt32(g.editPermission));
                cmdInsertUserPermission.Parameters.AddWithValue("@SensitivePermission", Convert.ToInt32(g.sensitiveInfoPermission));

                Lab3DBConnection.Open();
                cmdInsertUserPermission.ExecuteNonQuery();


            }

        }

        public static SqlDataReader Grant_UserReader(int GrantID)//reads grant user table in sql that is associated with grant value passed
        {

            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            SqlCommand cmdGrantRead = new SqlCommand();
            Console.WriteLine("GrantID: " + GrantID);
            cmdGrantRead.Parameters.Add(new SqlParameter("@GrantID", SqlDbType.Int) { Value = GrantID });
            cmdGrantRead.Connection = new SqlConnection(Lab3DBConnString);
            cmdGrantRead.CommandText = "Select * from Users join Grant_User ON users.UserID = Grant_User.UserID " +
                                       "where Grant_User.GrantID = @GrantID;";

            cmdGrantRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdGrantRead.ExecuteReader();
            return tempReader;
        }

        public static SqlDataReader User_GrantReader(int UserID)//reads grants from grant user table in sql that is associated with user value passed
        {

            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            SqlCommand cmdGrantRead = new SqlCommand();
            cmdGrantRead.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = UserID });
            cmdGrantRead.Connection = new SqlConnection(Lab3DBConnString);
            cmdGrantRead.CommandText = "select Grants.*, CONCAT(Users.firstName, ' ', Users.lastName) AS FacultyLead, Users.email AS FacultyLeadEmail" +
                " from Grants JOIN Grant_User ON Grants.GrantID = Grant_User.GrantID " +
                "join Users ON Users.UserID = Grants.FacultyLeadID " +
                "where Grant_User.userID = @UserID " +
                "ORDER BY CASE WHEN grantStatus = 'Active' THEN 1 WHEN grantStatus = 'Funded' THEN 2 WHEN grantStatus = 'Potential' THEN 3 WHEN grantStatus = 'Rejected' THEN 4 WHEN grantStatus = 'Archived' THEN 5 END ASC, Grants.dueDate ASC;";

            cmdGrantRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdGrantRead.ExecuteReader();
            return tempReader;
        }




        public static SqlDataReader readNonGrant_User(int GrantID)//When opening add user modal this will display users not associated with the grant
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            SqlCommand cmdDisplayUsers = new SqlCommand();
            cmdDisplayUsers.Connection = Lab3DBConnection;
            cmdDisplayUsers.Connection = new SqlConnection(Lab3DBConnString); ;
            cmdDisplayUsers.Parameters.Add(new SqlParameter("@GrantID", SqlDbType.Int) { Value = GrantID });

            cmdDisplayUsers.CommandText = "SELECT Users.* FROM Users " +
                "LEFT JOIN Grant_User ON Users.UserID = Grant_User.UserID" +
                " AND Grant_User.GrantID = @GrantID " +
                "WHERE Grant_User.UserID IS NULL;";



            cmdDisplayUsers.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdDisplayUsers.ExecuteReader();

            return tempReader;


        }


        public static void addGrantUser(int GrantID, int UserID)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            SqlCommand cmdAddUsers = new SqlCommand();
            cmdAddUsers.Connection = Lab3DBConnection;
            cmdAddUsers.Connection.ConnectionString = Lab3DBConnString;

            //add parameters grandID and userID
            cmdAddUsers.Parameters.Add(new SqlParameter("@GrantID", SqlDbType.Int) { Value = GrantID });
            cmdAddUsers.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = UserID });

            cmdAddUsers.CommandText = "INSERT INTO Grant_User (GrantID, UserID) VALUES (@GrantID, @UserID);";

            cmdAddUsers.Connection.Open(); // Open connection here, close in Model!

            cmdAddUsers.ExecuteNonQuery();
        }



        public static SqlDataReader SingleGrantReader(int GrantID)//reads grant table in sql
        {
            SqlCommand cmdGrantRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            cmdGrantRead.Parameters.Add(new SqlParameter("@GrantID", SqlDbType.Int) { Value = GrantID });
            cmdGrantRead.Connection = Lab3DBConnection;
            cmdGrantRead.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantRead.CommandText = "SELECT Grants.*, CONCAT(Users.firstName," +
                " ' ', Users.lastName) AS FacultyLead," +
                "Users.email AS FacultyLeadEmail FROM Grants JOIN Users ON Users.UserID = Grants.FacultyLeadID " +
                "WHERE Grants.GrantID = @GrantID;";
            cmdGrantRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdGrantRead.ExecuteReader();

            return tempReader;
        }


        public static void InsertReport(Report report, List<int> grantIDs, List<int> projectIDs, List<ReportSubject> subjects)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert Report
                        string insertReportQuery = @"
                    INSERT INTO Reports (ReportDate, ReportName, AuthorName) 
                    VALUES (@ReportDate, @ReportName, @AuthorName);
                    SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdInsertReport = new SqlCommand(insertReportQuery, connection, transaction);
                        cmdInsertReport.Parameters.AddWithValue("@ReportDate", report.ReportDate);
                        cmdInsertReport.Parameters.AddWithValue("@ReportName", report.ReportName);
                        Console.WriteLine("AuthorName: " + report.AuthorName);
                        cmdInsertReport.Parameters.AddWithValue("@AuthorName", report.AuthorName);

                        Console.WriteLine("Executing SQL Query: " + insertReportQuery);
                        Console.WriteLine($"Parameters: ReportDate = {report.ReportDate}, ReportName = {report.ReportName}, AuthorName = {report.AuthorName}");

                        object result = cmdInsertReport.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            throw new Exception("Failed to retrieve the ReportID after inserting the report.");
                        }

                        // Explicitly convert the result to an int
                        int reportID = Convert.ToInt32(result);
                        Console.WriteLine($"Report inserted successfully with ReportID = {reportID}");

                        // Insert ReportSubjects
                        string insertReportSubjectQuery = @"
                    INSERT INTO ReportSubjects (ReportID, SubjectTitle, SubjectText, GrantID, ProjectID) 
                    VALUES (@ReportID, @SubjectTitle, @SubjectText, @GrantID, @ProjectID);";

                        foreach (var subject in subjects)
                        {
                            Console.WriteLine($"Inserting ReportSubject: ReportID = {reportID}, SubjectTitle = {subject.SubjectTitle}, SubjectText = {subject.SubjectText}, GrantID = {subject.GrantID}, ProjectID = {subject.ProjectID}");

                            SqlCommand cmdInsertReportSubject = new SqlCommand(insertReportSubjectQuery, connection, transaction);
                            cmdInsertReportSubject.Parameters.AddWithValue("@ReportID", reportID);
                            cmdInsertReportSubject.Parameters.AddWithValue("@SubjectTitle", subject.SubjectTitle);
                            cmdInsertReportSubject.Parameters.AddWithValue("@SubjectText", subject.SubjectText);
                            cmdInsertReportSubject.Parameters.AddWithValue("@GrantID", subject.GrantID.HasValue ? (object)subject.GrantID.Value : DBNull.Value);
                            cmdInsertReportSubject.Parameters.AddWithValue("@ProjectID", subject.ProjectID.HasValue ? (object)subject.ProjectID.Value : DBNull.Value);

                            Console.WriteLine($"SQL Parameters: GrantID = {cmdInsertReportSubject.Parameters["@GrantID"].Value}, ProjectID = {cmdInsertReportSubject.Parameters["@ProjectID"].Value}");

                            cmdInsertReportSubject.ExecuteNonQuery();
                        }

                        // Commit transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if any error occurs
                        transaction.Rollback();
                        Console.WriteLine($"Error inserting report information: {ex.Message}");
                        throw;
                    }
                }
            }
        }







        public static int[] GetGrantStatusCounts()
        {
            int[] counts = new int[5]; // Order: Active, Potential, Funded, Archived, Rejected


            if (Lab3DBConnection.State == ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string query = "SELECT grantStatus, COUNT(*) AS Count FROM Grants GROUP BY grantStatus";

            using (SqlCommand cmd = new SqlCommand(query, Lab3DBConnection))
            {
                Lab3DBConnection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string status = reader["grantStatus"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);

                        switch (status)
                        {
                            case "Active":
                                counts[0] = count;
                                break;
                            case "Potential":
                                counts[1] = count;
                                break;
                            case "Funded":
                                counts[2] = count;
                                break;
                            case "Archived":
                                counts[3] = count;
                                break;
                            case "Rejected":
                                counts[4] = count;
                                break;
                        }
                    }
                }
            }
            return counts;
        }

        public static SqlDataReader AllReportReader()
        {
            SqlCommand cmdReportRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdReportRead.Connection = Lab3DBConnection;
            cmdReportRead.Connection.ConnectionString = Lab3DBConnString;

            // Updated query to include a LEFT JOIN with ReportSubjects to check for SubjectID
            cmdReportRead.CommandText = @"
        SELECT 
            Reports.ReportID,
            Reports.ReportName,
            Reports.ReportDate,
            Reports.AuthorName,
            ReportSubjects.SubjectID
        FROM 
            Reports
        LEFT JOIN 
            ReportSubjects ON Reports.ReportID = ReportSubjects.ReportID;";

            cmdReportRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdReportRead.ExecuteReader();

            return tempReader;
        }



        public static SqlDataReader AllPerformanceReportReader(int reportID)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Lab3DBConnection;
            cmd.Connection.ConnectionString = Lab3DBConnString;

            // Corrected SQL query
            cmd.CommandText = @"
        SELECT 
            PerformanceReport.PerformanceReportID,
            PerformanceReport.ReportID,
            PerformanceReport.Description,
            PerformanceReport.StartDate,
            PerformanceReport.EndDate,
            PerformanceReport.Funding,
            PerformanceReport.ProjectsCompleted,
            PerformanceReport.GrantsSubmitted,
            PerformanceReport.ProjectsWIP,
            PerformanceReport.PapersPublished,
            PerformanceReport.UnawardedFunding,
            PerformanceReport.PotentialGrants,
            PerformanceReport.AwardedGrants,
            PerformanceReport.ActiveGrants,
            PerformanceReport.RejectedGrants,
            PerformanceReport.ArchivedGrants,
            Reports.ReportName,
            Reports.AuthorName
        FROM 
            PerformanceReport
        INNER JOIN 
            Reports ON PerformanceReport.ReportID = Reports.ReportID
        WHERE 
            PerformanceReport.ReportID = @ReportID;";

            cmd.Parameters.AddWithValue("@ReportID", reportID);

            cmd.Connection.Open();
            return cmd.ExecuteReader();
        }





        public static PerformanceReport GetPerformanceReportData(DateTime startDate, DateTime endDate)
        {
            PerformanceReport report = new PerformanceReport();

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                connection.Open();

                // Total Funding
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(amount), 0) AS TotalFunding FROM Grants WHERE awardDate BETWEEN @StartDate AND @EndDate;", connection))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    report.Funding = Convert.ToDouble(cmd.ExecuteScalar());
                }

                // Projects Completed
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS ProjectsCompleted FROM Project WHERE projectStatus = 'Complete' AND dateCompleted BETWEEN @StartDate AND @EndDate;", connection))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    report.ProjectsCompleted = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Grants Submitted
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS GrantsSubmitted FROM Grants WHERE submissionDate BETWEEN @StartDate AND @EndDate;", connection))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    report.GrantsSubmitted = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Projects WIP
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS ProjectsWIP FROM Project WHERE projectStatus = 'In Progress';", connection))
                {
                    report.ProjectsWIP = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Papers Published
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS PapersPublished FROM Publishes WHERE DueDate BETWEEN @StartDate AND @EndDate;", connection))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    report.PapersPublished = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Unawarded Funding (Total funding amount for rejected grants)
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(amount), 0) AS UnawardedFunding FROM Grants WHERE grantStatus = 'RejectedGrants' AND submissionDate BETWEEN @StartDate AND @EndDate;", connection))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    report.UnawardedFunding = Convert.ToDouble(cmd.ExecuteScalar());
                }

                // Potential Grants
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS PotentialGrants FROM Grants WHERE grantStatus = 'PotentialGrants';", connection))
                {
                    report.PotentialGrants = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Awarded Grants
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS AwardedGrants FROM Grants WHERE grantStatus = 'AwardedGrants';", connection))
                {
                    report.AwardedGrants = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Active Grants
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS ActiveGrants FROM Grants WHERE grantStatus = 'ActiveGrants';", connection))
                {
                    report.ActiveGrants = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Rejected Grants
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS RejectedGrants FROM Grants WHERE grantStatus = 'RejectedGrants';", connection))
                {
                    report.RejectedGrants = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Archived Grants
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(COUNT(*), 0) AS ArchivedGrants FROM Grants WHERE grantStatus = 'ArchivedGrants';", connection))
                {
                    report.ArchivedGrants = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return report;
        }







        public static void InsertPerformanceReport(PerformanceReport report)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Insert into Reports table (if ReportID is not null)
                        int? reportID = null;
                        if (report.ReportID == 0) // If no ReportID is provided, insert a new Report
                        {
                            string insertReportQuery = @"
                        INSERT INTO Reports (ReportDate, ReportName, AuthorName) 
                        VALUES (@ReportDate, @ReportName, @AuthorName);
                        SELECT SCOPE_IDENTITY();";

                            SqlCommand cmdInsertReport = new SqlCommand(insertReportQuery, connection, transaction);
                            cmdInsertReport.Parameters.AddWithValue("@ReportDate", report.StartDate); // Use StartDate as the report date
                            cmdInsertReport.Parameters.AddWithValue("@ReportName", report.PerformanceReportName ?? "Performance Report");
                            cmdInsertReport.Parameters.AddWithValue("@AuthorName", report.AuthorName ?? "Unknown");

                            object result = cmdInsertReport.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                throw new Exception("Failed to retrieve the ReportID after inserting into the Reports table.");
                            }

                            reportID = Convert.ToInt32(result);
                        }
                        else
                        {
                            reportID = report.ReportID; // Use the provided ReportID
                        }

                        // Step 2: Insert into PerformanceReport table
                        string insertPerformanceReportQuery = @"
                    INSERT INTO PerformanceReport 
                    (ReportID, Description, StartDate, EndDate, Funding, ProjectsCompleted, GrantsSubmitted, ProjectsWIP, PapersPublished, 
                     UnawardedFunding, PotentialGrants, AwardedGrants, ActiveGrants, RejectedGrants, ArchivedGrants)
                    VALUES 
                    (@ReportID, @Description, @StartDate, @EndDate, @Funding, @ProjectsCompleted, @GrantsSubmitted, @ProjectsWIP, @PapersPublished, 
                     @UnawardedFunding, @PotentialGrants, @AwardedGrants, @ActiveGrants, @RejectedGrants, @ArchivedGrants);";

                        SqlCommand cmdInsertPerformanceReport = new SqlCommand(insertPerformanceReportQuery, connection, transaction);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@ReportID", reportID);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@Description", report.Description ?? (object)DBNull.Value);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@StartDate", report.StartDate);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@EndDate", report.EndDate);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@Funding", report.Funding);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@ProjectsCompleted", report.ProjectsCompleted);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@GrantsSubmitted", report.GrantsSubmitted);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@ProjectsWIP", report.ProjectsWIP);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@PapersPublished", report.PapersPublished);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@UnawardedFunding", report.UnawardedFunding);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@PotentialGrants", report.PotentialGrants);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@AwardedGrants", report.AwardedGrants);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@ActiveGrants", report.ActiveGrants);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@RejectedGrants", report.RejectedGrants);
                        cmdInsertPerformanceReport.Parameters.AddWithValue("@ArchivedGrants", report.ArchivedGrants);

                        cmdInsertPerformanceReport.ExecuteNonQuery();

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        transaction.Rollback();
                        Console.WriteLine($"Error inserting performance report: {ex.Message}");
                        throw;
                    }
                }
            }
        }




        public static SqlDataReader SingleReportReader(int reportID)//reads progress report data
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string query = @"
        SELECT 
            Reports.ReportID,
            Reports.ReportName,
            Reports.ReportDate,
            Reports.AuthorName,
            ReportSubjects.SubjectTitle,
            ReportSubjects.SubjectText,
            ReportSubjects.GrantID,
            ReportSubjects.ProjectID,
            Grants.GrantName,
            Grants.businessName AS GrantBusinessName,
            Grants.category AS GrantCategory,
            Grants.dueDate AS GrantDueDate,
            Grants.grantStatus AS GrantStatus,
            Grants.amount AS GrantAmount,
            Project.ProjectID,
            Project.ProjectName,
            Project.ProjectStatus,
            Project.dueDate AS ProjectDueDate,
            Project.DateCreated AS ProjectCreatedDate,
            Project.DateCompleted AS ProjectCompletedDate
        FROM 
            Reports
        LEFT JOIN 
            ReportSubjects ON Reports.ReportID = ReportSubjects.ReportID
        LEFT JOIN 
            Grants ON ReportSubjects.GrantID = Grants.GrantID
        LEFT JOIN 
            Project ON ReportSubjects.ProjectID = Project.ProjectID
        WHERE 
            Reports.ReportID = @ReportID;";

            SqlCommand cmdReportRead = new SqlCommand(query, Lab3DBConnection);
            cmdReportRead.Parameters.AddWithValue("@ReportID", reportID);

            cmdReportRead.Connection.Open();
            SqlDataReader tempReader = cmdReportRead.ExecuteReader();

            return tempReader;
        }






        public static void UploadFile(IFormFile formFile)
        {
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }
                if (formFile.Length > 0)
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        formFile.CopyTo(memoryStream);
                        var fileData = new FileUploads
                        {
                            FileName = formFile.FileName,
                            FileData = memoryStream.ToArray(),
                            UploadDate = DateTime.Now
                        };


                    }
                    SqlCommand cmdInsert = new SqlCommand();
                    cmdInsert.Connection = Lab3DBConnection;
                    cmdInsert.Connection.ConnectionString = Lab3DBConnString;
                    //cmdInsert.CommandText = ;
                    cmdInsert.Connection.Open();
                    cmdInsert.ExecuteNonQuery();



                }
            }
        }


        //retrieving the user profile picture
        public static User? GetUserInfoById(int userId)
        {
            User? user = null;

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                string query = @"SELECT userID, firstName, lastName, email, phoneNumber, UserType, ProfileImageFileName 
                         FROM Users WHERE userID = @userID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                userID = reader.GetInt32(reader.GetOrdinal("userID")),
                                firstName = reader.GetString(reader.GetOrdinal("firstName")),
                                lastName = reader.GetString(reader.GetOrdinal("lastName")),
                                email = reader.GetString(reader.GetOrdinal("email")),
                                phone = reader["phoneNumber"] as string,
                                UserType = reader["UserType"] as int?,
                                ProfileImageFileName = reader["ProfileImageFileName"] as string
                            };
                            Console.WriteLine($"User retrieved: {user.firstName} {user.lastName}");
                        }
                        else
                        {
                            Console.WriteLine($"No user found with userID: {userId}");
                        }
                    }
                }
            }

            return user;
        }


        //Publish
        public static List<Publish> GetAllPublishes()
        {
            List<Publish> list = new();
            using SqlConnection conn = new SqlConnection(Lab3DBConnString);
            string query = "SELECT * FROM Publishes";
            SqlCommand cmd = new(query, conn);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Publish
                {
                    PublishID = (int)reader["PublishID"],
                    JournalTitle = reader["JournalTitle"].ToString(),
                    DueDate = reader["DueDate"] as DateTime?,
                    Requirements = reader["Requirements"].ToString(),
                    Authors = reader["Authors"].ToString(),
                    Status = reader["Status"].ToString(),
                    ReferenceCount = (int)reader["ReferenceCount"],
                    FileName = reader["FileName"].ToString()


                });
            }
            return list;
        }

        public static void InsertPublish(Publish p)
        {
            using SqlConnection conn = new SqlConnection(Lab3DBConnString);
            string query = @"INSERT INTO Publishes (JournalTitle, DueDate, Requirements, Authors, Status, ReferenceCount, FileName)
                     VALUES (@JournalTitle, @DueDate, @Requirements, @Authors, @Status, @ReferenceCount, @FileName)";
            SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@DueDate", p.DueDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Requirements", p.Requirements ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Authors", p.Authors ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", p.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ReferenceCount", p.ReferenceCount);
            cmd.Parameters.AddWithValue("@JournalTitle", p.JournalTitle ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FileName", p.FileName ?? (object)DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }



        public static void AddProfileImage(User user)
        {
            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                string query = "UPDATE Users SET ProfileImageFileName = @ProfileImageFileName WHERE userID = @userID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProfileImageFileName", user.ProfileImageFileName);
                cmd.Parameters.AddWithValue("@userID", user.userID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

            /*Lab3DBConnection.Open();
            cmd.ExecuteNonQuery();
            Lab3DBConnection.Close();*/
        }



    }



