using System.Data.SqlClient;

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
            cmdProjectRead.CommandText = "SELECT * FROM Project";
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
            cmdGrantRead.CommandText = "SELECT * FROM [Grants]";
            cmdGrantRead.Connection.Open(); // Open connection here, close in Model!

            SqlDataReader tempReader = cmdGrantRead.ExecuteReader();

            return tempReader;
        }
    }
}
