using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;

namespace Lab1484.Pages.DB
{
    public class DBClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection OrgGrantDBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly String? OrgGrantDBConnString =
            "Server=LocalHost;Database=OrgGrant;Trusted_Connection=True";

        //Connection Methods:

        //Basic Product Reader
        public static SqlDataReader ProjectReader()
        {
            SqlCommand cmdProjectRead = new SqlCommand();//Make new sqlCommand object
            if (OrgGrantDBConnection.State == System.Data.ConnectionState.Open)
            {
                OrgGrantDBConnection.Close();
            }
            cmdProjectRead.Connection = OrgGrantDBConnection;
            cmdProjectRead.Connection.ConnectionString = OrgGrantDBConnString;
            cmdProjectRead.CommandText = "Select Project.*, Concat(Users.firstName, ' ', Users.lastName) AS AdminName " +
                "from Project " +
                "join Users ON Users.UserID = Project.ProjectAdminID; ";
            cmdProjectRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdProjectRead.ExecuteReader();

            return tempReader;
            cmdProjectRead.Connection.Close();
        }
        public static SqlDataReader GrantReader()
        {
            SqlCommand cmdGrantRead = new SqlCommand();
            if (OrgGrantDBConnection.State == System.Data.ConnectionState.Open)
            {
                OrgGrantDBConnection.Close();
            }
            cmdGrantRead.Connection = OrgGrantDBConnection;
            cmdGrantRead.Connection.ConnectionString = OrgGrantDBConnString;
            cmdGrantRead.CommandText = "Select Grants.*, Concat(Users.firstName, ' ', Users.lastName) AS FacultyLead " +
                "from Grants " +
                "join Users ON Users.UserID = Grants.FacultyLeadID; ";
            cmdGrantRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdGrantRead.ExecuteReader();

            return tempReader;
        }

       
        public static void InsertUser(User p)
        {

            if (OrgGrantDBConnection.State == System.Data.ConnectionState.Open)
            {
                OrgGrantDBConnection.Close();
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
            cmdProjectRead.Connection = OrgGrantDBConnection;
            cmdProjectRead.Connection.ConnectionString =
            OrgGrantDBConnString;
            cmdProjectRead.CommandText = sqlQuery;
            cmdProjectRead.Connection.Open();
            cmdProjectRead.ExecuteNonQuery();

        }
        public static SqlDataReader AdminReader()
        {
            SqlCommand cmdAdminRead = new SqlCommand();
            if (OrgGrantDBConnection.State == System.Data.ConnectionState.Open)
            {
                OrgGrantDBConnection.Close();
            }
            cmdAdminRead.Connection = OrgGrantDBConnection;
            cmdAdminRead.Connection.ConnectionString = OrgGrantDBConnString;
            cmdAdminRead.CommandText = "SELECT Users.userID, Users.firstName, Users.lastName " +
                "FROM Users " +
                "WHERE Users.userType=0;";
            cmdAdminRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdAdminRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        public static SqlDataReader EmployeeReader()
        {
            SqlCommand cmdEmployeeRead = new SqlCommand();
            if (OrgGrantDBConnection.State == System.Data.ConnectionState.Open)
            {
                OrgGrantDBConnection.Close();
            }
            cmdEmployeeRead.Connection = OrgGrantDBConnection;
            cmdEmployeeRead.Connection.ConnectionString = OrgGrantDBConnString;
            cmdEmployeeRead.CommandText = "SELECT Users.userID, Users.firstName, Users.lastName " +
                "FROM Users " +
                "WHERE Users.userType=2;";
            cmdEmployeeRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdEmployeeRead.ExecuteReader();

            return tempReader;
            //cmdAdminRead.Connection.Close();
        }

        public static void InsertProject(Project p)
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
            cmdProjectRead.Connection = OrgGrantDBConnection;
            cmdProjectRead.Connection.ConnectionString =
            OrgGrantDBConnString;
            cmdProjectRead.CommandText = sqlQuery;
            cmdProjectRead.Connection.Open();
            cmdProjectRead.ExecuteNonQuery();
        }
    }
}

