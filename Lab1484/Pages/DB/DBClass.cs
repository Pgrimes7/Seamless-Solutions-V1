using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.InteropServices;
using Lab1484.Pages.DataClasses;
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


        // Connection String - How to find and connect to DB
        private static readonly String? Lab3DBConnString =
            "Server=LocalHost;Database=Lab3;Trusted_Connection=True";

        // A second connection String
        // For Hashed Passwords
        private static readonly String? AuthConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";

        //Connection Methods:

        //Basic Project Reader
        public static SqlDataReader ProjectReader()
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;
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
        public static void UpdateProject(Project p)
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
            cmdProjectRead.CommandText = "SELECT Tasks.*, Project.ProjectName " +
                "FROM Tasks " +
                "JOIN Project ON Project.ProjectID = Tasks.ProjectID;";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }

        //Insert Task
        public static void InsertTask(ProjTask t)
        {
            {
                if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
                {
                    Lab3DBConnection.Close();
                }

                string sqlQuery = "INSERT INTO Tasks (ProjectID, taskDescription, dueDate) VALUES (@ProjectID, @taskDescription, @dueDate);";

                SqlCommand cmdTaskInsert = new SqlCommand();
                cmdTaskInsert.Connection = Lab3DBConnection;
                cmdTaskInsert.Connection.ConnectionString = Lab3DBConnString;
                cmdTaskInsert.CommandText = sqlQuery;
                cmdTaskInsert.Parameters.AddWithValue("@ProjectID", t.ProjectID);
                cmdTaskInsert.Parameters.AddWithValue("@taskDescription", t.taskDescription);
                cmdTaskInsert.Parameters.AddWithValue("@dueDate", t.dueDate);
                cmdTaskInsert.Connection.Open();
                cmdTaskInsert.ExecuteNonQuery();
            }
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
                    "where grantName LIKE @SearchQuery OR Concat(Users.firstName, ' ', Users.lastName) LIKE @SearchQuery OR Users.email LIKE @SearchQuery OR amount LIKE @SearchQuery OR dueDate LIKE @SearchQuery OR grantStatus LIKE @SearchQuery OR businessName LIKE @SearchQuery OR category LIKE @SearchQuery; ";

                cmdGrantRead.Parameters.AddWithValue("@SearchQuery", "%" + SearchQuery + "%");

            }

            else
            {
                cmdGrantRead.CommandText = "Select Grants.*, Concat(Users.firstName, ' ', Users.lastName) AS FacultyLead, Users.email AS FacultyLeadEmail " +
                    "from Grants " +
                    "join Users ON Users.UserID = Grants.FacultyLeadID; ";
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






        public static void InsertGrant(Grant g)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string sqlQuery = @"INSERT INTO Grants (FacultyLeadID, BusinessPartnerID, businessName,
                        category, dueDate, grantStatus, amount, grantName)
                        VALUES (@FacultyLeadID, @businessPartnerID, @businessName,
                        @category, @dueDate, @grantStatus, @amount, @grantName);";

            SqlCommand cmdGrantInsert = new SqlCommand();
            cmdGrantInsert.Connection = Lab3DBConnection;
            cmdGrantInsert.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantInsert.CommandText = sqlQuery;
            cmdGrantInsert.Parameters.AddWithValue("@FacultyLeadID", g.FacultyLeadID);
            cmdGrantInsert.Parameters.AddWithValue("@businessPartnerID", g.BusinessPartnerID);
            cmdGrantInsert.Parameters.AddWithValue("@businessName", g.businessName);
            cmdGrantInsert.Parameters.AddWithValue("@category", g.category);
            cmdGrantInsert.Parameters.AddWithValue("@dueDate", g.dueDate);
            cmdGrantInsert.Parameters.AddWithValue("@grantStatus", g.grantStatus);
            cmdGrantInsert.Parameters.AddWithValue("@amount", g.amount);
            cmdGrantInsert.Parameters.AddWithValue("@grantName", g.grantName);

            cmdGrantInsert.Connection.Open();
            cmdGrantInsert.ExecuteNonQuery();
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

        //Messages methods:
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
                        IsRead = (int)reader["IsRead"]
                    };
                    messages.Add(message); // Add the message instance to the list
                }
            }
            return messages; // Return the list of messages
        }

        public static List<MessagesModel> GetUserSentMessages(string sender)
        {
            List<MessagesModel> messages = new List<MessagesModel>();
            string query = "SELECT * FROM Messages WHERE Sender = @Sender ORDER BY TimeStamp DESC";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Sender", sender);

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
                        IsRead = (int)reader["IsRead"]
                    };
                    messages.Add(message); // Add the message instance to the list
                }
            }
            return messages; // Return the list of messages
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

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
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


        public static void CreateHashedUser(User p)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }



            string userInsertQuery = @"
             INSERT INTO Users (userType, firstName, lastName, email, phoneNumber)
             VALUES (@UserType, @firstName, @lastName, @email, @phoneNumber);

             SELECT SCOPE_IDENTITY();";

            SqlCommand cmdUserInsert = new SqlCommand();
            cmdUserInsert.Connection = Lab3DBConnection;
            cmdUserInsert.CommandText = userInsertQuery;

            cmdUserInsert.Parameters.AddWithValue("@UserType", p.UserType);
            cmdUserInsert.Parameters.AddWithValue("@firstName", p.firstName);
            cmdUserInsert.Parameters.AddWithValue("@lastName", p.lastName);
            cmdUserInsert.Parameters.AddWithValue("@email", p.email);
            cmdUserInsert.Parameters.AddWithValue("@phoneNumber", p.phone);

            cmdUserInsert.Connection.Open();


            int userID = Convert.ToInt32(cmdUserInsert.ExecuteScalar());

            string newHashedCredsQuery = @"
            INSERT INTO HashedCredentials (UserID,Username,Password)
            VALUES (@UserID, @Username, @Password);";

            SqlCommand cmdNewHashed = new SqlCommand();
            cmdNewHashed.Connection = new SqlConnection(AuthConnString);
            cmdNewHashed.CommandText = newHashedCredsQuery;
            cmdNewHashed.Parameters.AddWithValue("@Username", p.username);
            cmdNewHashed.Parameters.AddWithValue("@UserID", userID);
            cmdNewHashed.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(p.password));
            cmdNewHashed.Connection.Open();

            cmdNewHashed.ExecuteNonQuery();

        }


        public static void UpdateHashedUser(UserUpdate p)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }

            string userUpdatetQuery = @"
             UPDATE Users
             SET userType = @UserType, firstName = @FirstName, lastName = @LastName, email = @Email, phoneNumber = @Phone
             WHERE UserID = @UserID;";

            SqlCommand cmdUserUpdate = new SqlCommand();
            cmdUserUpdate.Connection = Lab3DBConnection;
            cmdUserUpdate.CommandText = userUpdatetQuery;

            cmdUserUpdate.Parameters.AddWithValue("@UserID", p.UserID);
            cmdUserUpdate.Parameters.AddWithValue("@UserType", p.UserType);
            cmdUserUpdate.Parameters.AddWithValue("@firstName", p.FirstName);
            cmdUserUpdate.Parameters.AddWithValue("@lastName", p.LastName);
            cmdUserUpdate.Parameters.AddWithValue("@email", p.Email);
            cmdUserUpdate.Parameters.AddWithValue("@phoneNumber", p.Phone);

            cmdUserUpdate.Connection.Open();

            cmdUserUpdate.ExecuteNonQuery();
            cmdUserUpdate.Connection.Close();

            /*int userID = Convert.ToInt32(cmdUserUpdate.ExecuteScalar());*/

            /**string updatedHashedCredsQuery = @"
            UPDATE HashedCredentials
            Set Username = @Username, Password = @Password
            WHERE UserID = @UserID;";

            SqlCommand cmdUpdatedHashed = new SqlCommand();
            cmdUpdatedHashed.Connection = new SqlConnection(AuthConnString);
            cmdUpdatedHashed.CommandText = updatedHashedCredsQuery;
            cmdUpdatedHashed.Parameters.AddWithValue("@Username", p.Username);
            cmdUpdatedHashed.Parameters.AddWithValue("@UserID", p.UserID);
            cmdUpdatedHashed.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(p.Password));
            cmdUpdatedHashed.Connection.Open();**/

            /*cmdUpdatedHashed.ExecuteNonQuery();*/
            /**cmdUpdatedHashed.Connection.Open();
            cmdUpdatedHashed.ExecuteNonQuery();
            cmdUpdatedHashed.Connection.Close();**/

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

        public static void InsertNote(Note n)
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
        }
        public static int checkUserType(HttpContext httpContext)//Calls httpContext to pull UserID 
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string findUserType = "getUserType"; // Stored procedure to find userType given userID from login method

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
                "where Grant_User.userID = @UserID;";

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

            //unsure if work will come back to this
            using (SqlConnection connection = new SqlConnection(Lab3DBConnString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert Report
                        string insertReportQuery = "INSERT INTO Reports (ReportDate, ReportName) OUTPUT INSERTED.ReportID VALUES (@ReportDate, @ReportName);";
                        SqlCommand cmdInsertReport = new SqlCommand(insertReportQuery, connection, transaction);
                        cmdInsertReport.Parameters.AddWithValue("@ReportDate", report.ReportDate);
                        cmdInsertReport.Parameters.AddWithValue("@ReportName", report.ReportName);
                        int reportID = (int)cmdInsertReport.ExecuteScalar();

                        // Insert ReportGrants
                        string insertReportGrantQuery = "INSERT INTO ReportGrants (ReportID, GrantID) VALUES (@ReportID, @GrantID);";
                        foreach (int grantID in grantIDs)
                        {
                            SqlCommand cmdInsertReportGrant = new SqlCommand(insertReportGrantQuery, connection, transaction);
                            cmdInsertReportGrant.Parameters.AddWithValue("@ReportID", reportID);
                            cmdInsertReportGrant.Parameters.AddWithValue("@GrantID", grantID);
                            cmdInsertReportGrant.ExecuteNonQuery();
                        }

                        // Insert ReportProjects
                        string insertReportProjectQuery = "INSERT INTO ReportProjects (ReportID, ProjectID) VALUES (@ReportID, @ProjectID);";
                        foreach (int projectID in projectIDs)
                        {
                            SqlCommand cmdInsertReportProject = new SqlCommand(insertReportProjectQuery, connection, transaction);
                            cmdInsertReportProject.Parameters.AddWithValue("@ReportID", reportID);
                            cmdInsertReportProject.Parameters.AddWithValue("@ProjectID", projectID);
                            cmdInsertReportProject.ExecuteNonQuery();
                        }

                        // Insert ReportSubjects
                        string insertReportSubjectQuery = "INSERT INTO ReportSubjects (ReportID, SubjectTitle, SubjectText) VALUES (@ReportID, @SubjectTitle, @SubjectText);";
                        foreach (var subject in subjects)
                        {
                            SqlCommand cmdInsertReportSubject = new SqlCommand(insertReportSubjectQuery, connection, transaction);
                            cmdInsertReportSubject.Parameters.AddWithValue("@ReportID", reportID);
                            cmdInsertReportSubject.Parameters.AddWithValue("@SubjectTitle", subject.SubjectTitle);
                            cmdInsertReportSubject.Parameters.AddWithValue("@SubjectText", subject.SubjectText);
                            cmdInsertReportSubject.ExecuteNonQuery();
                        }

                        // Commit transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if any error occurs
                        transaction.Rollback();
                        throw new Exception("Error inserting report information: " + ex.Message);
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



    }
}

