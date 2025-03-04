using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Microsoft.AspNetCore.Identity;

namespace Lab1484.Pages.DB
{
    public class DBClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection Lab2DBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? Lab2DBConnString =
            "Server=LocalHost;Database=Lab2;Trusted_Connection=True";

        //Connection Methods:

        //Basic Product Reader
        public static SqlDataReader ProjectReader()
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdProjectRead.Connection = Lab2DBConnection;
            cmdProjectRead.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdGrantRead.Connection = Lab2DBConnection;
            cmdGrantRead.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdAdminRead.Connection = Lab2DBConnection;
            cmdAdminRead.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdEmployeeRead.Connection = Lab2DBConnection;
            cmdEmployeeRead.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdEmployeeRead.Connection = Lab2DBConnection;
            cmdEmployeeRead.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdPartnerRead.Connection = Lab2DBConnection;
            cmdPartnerRead.Connection.ConnectionString = Lab2DBConnString;
            cmdPartnerRead.CommandText = "SELECT BusinessPartner.BusinessPartnerID, BusinessPartner.firstName," +
                " BusinessPartner.lastName " +
                "FROM BusinessPartner;";
            cmdPartnerRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdPartnerRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        public static void InsertProject(Project p)//inserts new project into DB
        {
            String sqlQuery = "INSERT INTO Project" +
            "(ProjectAdminID, projectStatus, dateCreated, dateCompleted, dueDate, projectName) VALUES('";
            sqlQuery += p.ProjectAdminID + "',";
            sqlQuery += p.ProjectStatus + ",'";
            sqlQuery += p.DateCreated + "')";
            sqlQuery += p.DateCompleted + "')";
            sqlQuery += p.DateDue + "')";
            sqlQuery += p.ProjectName + "')";
            SqlCommand cmdProjectRead = new SqlCommand();
            cmdProjectRead.Connection = Lab2DBConnection;
            cmdProjectRead.Connection.ConnectionString =
            Lab2DBConnString;
            cmdProjectRead.CommandText = sqlQuery;
            cmdProjectRead.Connection.Open();
            cmdProjectRead.ExecuteNonQuery();
        }

        //Inserts User into DB
        public static void InsertUser(User p)//inserts user into sql
        {

            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            string sqlQuery = "INSERT INTO Users (userType, firstName, lastName, email, phoneNumber) VALUES (";
            sqlQuery += p.UserType + ", '"
                + p.firstName
                + "', '"
                + p.lastName
                + "', '"
                + p.email + "', '"
                + p.phone + "');";



            SqlCommand cmdProjectRead = new SqlCommand();
            cmdProjectRead.Connection = Lab2DBConnection;
            cmdProjectRead.Connection.ConnectionString =
            Lab2DBConnString;
            cmdProjectRead.CommandText = sqlQuery;
            cmdProjectRead.Connection.Open();
            cmdProjectRead.ExecuteNonQuery();

        }






        //Inserts new grant into DB
        public static void InsertGrant(Grant g)
        {
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            string sqlQuery = "INSERT INTO Grants (FacultyLeadID, BusinessPartnerID, businessName," +
                " category, submissionDate, awardDate, grantStatus, amount)" +
                " VALUES (";
            sqlQuery += g.FacultyLeadID + ", '"
                + g.BusinessPartnerID
                + "', '"
                + g.businessName + "', '"
                + g.category + "', '"
                + g.submissionDate + "', '"
                + g.awardDate + "', '"
                + g.grantStatus + "', '"
                + g.amount + "');";

            SqlCommand cmdGrantInsert = new SqlCommand();
            cmdGrantInsert.Connection = Lab2DBConnection;
            cmdGrantInsert.Connection.ConnectionString = Lab2DBConnString;
            cmdGrantInsert.CommandText = sqlQuery;
            cmdGrantInsert.Connection.Open();
            cmdGrantInsert.ExecuteNonQuery();



        }





        public static SqlDataReader EmployeeProjectReader()
        {
            SqlCommand cmdEmployeeProjectRead = new SqlCommand();
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdEmployeeProjectRead.Connection = Lab2DBConnection;
            cmdEmployeeProjectRead.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            // This method expects to receive an SQL SELECT
            // query that uses the COUNT command.

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab2DBConnection;
            cmdLogin.Connection.ConnectionString = Lab2DBConnString;
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            string loginQuery =
                "SELECT COUNT(*) FROM Credentials where Username = @Username and Password = @Password";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = Lab2DBConnection;
            cmdLogin.Connection.ConnectionString = Lab2DBConnString;

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
        public static int GetUnreadMessagesCount(string receiver)
        {
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }

            // Query to count unread messages for the given receiver
            string query = "SELECT COUNT(*) FROM Messages WHERE Receiver = @Receiver AND IsRead = 0";

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Lab2DBConnection;
            cmd.Connection.ConnectionString = Lab2DBConnString;
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

            using (SqlConnection conn = new SqlConnection(Lab2DBConnString))  
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

            using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
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
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdMessageRead.Connection = Lab2DBConnection;
            cmdMessageRead.Connection.ConnectionString = Lab2DBConnString;
            cmdMessageRead.CommandText = "Select Messages.* FROM Messages;";
            cmdMessageRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdMessageRead.ExecuteReader();

            return tempReader;
            cmdMessageRead.Connection.Close();
        }

        public static SqlDataReader CredentialsReader()
        {
            SqlCommand cmdCredentialsRead = new SqlCommand();//Make new sqlCommand object
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            cmdCredentialsRead.Connection = Lab2DBConnection;
            cmdCredentialsRead.Connection.ConnectionString = Lab2DBConnString;
            cmdCredentialsRead.CommandText = "Select Credentials.Username FROM Credentials;";
            cmdCredentialsRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdCredentialsRead.ExecuteReader();

            return tempReader;
            cmdCredentialsRead.Connection.Close();
        }

        //Inserts new message into DB
        public static void InsertMessage(Message m)
        {
            if (Lab2DBConnection.State == System.Data.ConnectionState.Open)
            {
                Lab2DBConnection.Close();
            }
            string sqlQuery = "INSERT INTO Messages (Sender, Receiver, Content)" +
                " VALUES (@Sender, @Receiver, @Content)";
            
            using (SqlCommand cmdMessageInsert = new SqlCommand(sqlQuery, Lab2DBConnection))
            {
                cmdMessageInsert.Parameters.AddWithValue("@Sender", m.Sender);
                cmdMessageInsert.Parameters.AddWithValue("@Receiver", m.Receiver);
                cmdMessageInsert.Parameters.AddWithValue("@Content", m.Content);

                cmdMessageInsert.Connection.Open();
                cmdMessageInsert.ExecuteNonQuery();
            }
        }
    }
}

