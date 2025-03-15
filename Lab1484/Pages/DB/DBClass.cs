using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
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

        //Basic Product Reader
        public static SqlDataReader ProjectReader()
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab3DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab3DBConnString;
            cmdProjectRead.CommandText = "Select Project.*, Concat(Users.firstName, ' ', Users.lastName) AS AdminName " +
                "from Project " +
                "join Users ON Users.UserID = Project.ProjectAdminID; ";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }
        public static SqlDataReader GrantReader()//reads grant table in sql
        {
            SqlCommand cmdGrantRead = new SqlCommand();
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            cmdGrantRead.Connection = Lab3DBConnection;
            cmdGrantRead.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantRead.CommandText = "Select Grants.*, Concat(Users.firstName, ' ', Users.lastName) AS FacultyLead " +
                "from Grants " +
                "join Users ON Users.UserID = Grants.FacultyLeadID; ";
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






        //Inserts new grant into DB
        public static void InsertGrant(Grant g)
        {
            if (Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab3DBConnection.Close();
            }
            string sqlQuery = @"INSERT INTO Grants (FacultyLeadID, BusinessPartnerID, businessName," +
                " category, submissionDate, awardDate, grantStatus, amount)" +
                " VALUES (@FacultyLeadID, @businessPartnerID, @businessName, @category, @submissionDate, @awardDate, @grantStatus, @amount);";


            SqlCommand cmdGrantInsert = new SqlCommand();
            cmdGrantInsert.Connection = Lab3DBConnection;
            cmdGrantInsert.Connection.ConnectionString = Lab3DBConnString;
            cmdGrantInsert.CommandText = sqlQuery;
            cmdGrantInsert.Parameters.AddWithValue("@FacultyLeadID", g.FacultyLeadID);
            cmdGrantInsert.Parameters.AddWithValue("@businessPartnerID", g.BusinessPartnerID);
            cmdGrantInsert.Parameters.AddWithValue("@businessName", g.businessName);
            cmdGrantInsert.Parameters.AddWithValue("@category", g.category);
            cmdGrantInsert.Parameters.AddWithValue("@submissionDate", g.submissionDate);
            cmdGrantInsert.Parameters.AddWithValue("@awardDate", g.awardDate);
            cmdGrantInsert.Parameters.AddWithValue("@grantStatus", g.grantStatus);
            cmdGrantInsert.Parameters.AddWithValue("@amount", g.amount);

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
                        IsRead = (bool)reader["IsRead"]
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
                        IsRead = (bool)reader["IsRead"]
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
            cmdCredentialsRead.Connection.ConnectionString = Lab3DBConnString;
            cmdCredentialsRead.CommandText = "Select Credentials.Username FROM Credentials;";
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
                " VALUES (@Sender, @Receiver, @Content)";

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
                    httpcontext.Session.SetString("userID", userID.ToString());//used co pilot for calling httpcontext in this seting
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



        //Login methods for existing users that were made before password hashing was implemented:

        //To be called from the login page to check if migration is needed.
        public static void MigrateUserPasswords()
        {
            if (!IsFirstRun())
            {
                PerformMigration();
                MarkMigrationAsCompleted();
            }
        }

        //Migration logic - extracts plain-text passwords from the Credentials table and hashes them into HashedCredentials.
        private static void PerformMigration()
        {
            using (SqlConnection lab3Conn = new SqlConnection(Lab3DBConnString))
            using (SqlConnection authConn = new SqlConnection(AuthConnString))
            {
                lab3Conn.Open();
                authConn.Open();

                string getUsersQuery = "SELECT UserID, Username, Password FROM Credentials";
                SqlCommand getUsersCmd = new SqlCommand(getUsersQuery, lab3Conn);
                SqlDataReader reader = getUsersCmd.ExecuteReader();

                while (reader.Read())
                {
                    int userId = reader.GetInt32(0);
                    string username = reader.GetString(1);
                    string plainTextPassword = reader.GetString(2);

                    string hashedPassword = PasswordHash.HashPassword(plainTextPassword);

                    string insertHashedQuery = @"
                    INSERT INTO HashedCredentials (UserID, Username, Password) 
                    VALUES (@UserID, @Username, @Password)";

                    using (SqlCommand insertHashedCmd = new SqlCommand(insertHashedQuery, authConn))
                    {
                        insertHashedCmd.Parameters.AddWithValue("@UserID", userId);
                        insertHashedCmd.Parameters.AddWithValue("@Username", username);
                        insertHashedCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        insertHashedCmd.ExecuteNonQuery();
                    }
                }
                reader.Close();
                Console.WriteLine("Password migration complete.");
            }
        }

        //Check if migration flag exists/if the migration has been completed
        private static bool IsFirstRun()
        {
            string checkFlagQuery = "SELECT COUNT(1) FROM MigrationSettings WHERE SettingKey = 'migration_completed' AND SettingValue = 'true'";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(checkFlagQuery, conn);
                conn.Open();

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }

        //Set the flag to mark migration as completed
        private static void MarkMigrationAsCompleted()
        {
            string updateFlagQuery = "UPDATE MigrationSettings SET SettingValue = 'true' WHERE SettingKey = 'migration_completed'";

            using (SqlConnection conn = new SqlConnection(Lab3DBConnString))
            {
                SqlCommand cmd = new SqlCommand(updateFlagQuery, conn);
                conn.Open();

                cmd.ExecuteNonQuery();
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
        public static int checkUserType(HttpContext httpContext)//used copilot to call httpContext here
        {
            string findUserType = "getUserType"; // Stored procedure to find userType given userID from login method

            using (SqlCommand cmdCheckUser = new SqlCommand())
            {
                cmdCheckUser.Connection = Lab3DBConnection;

                cmdCheckUser.CommandText = findUserType;
                cmdCheckUser.CommandType = CommandType.StoredProcedure;

                string userID = httpContext.Session.GetString("userID");//Used AI to learn how to set the userID in the session 


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
    }
}
            





