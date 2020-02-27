using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProjectTemplate
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]

    public class ProjectServices : System.Web.Services.WebService
    {
        ////////////////////////////////////////////////////////////////////////
        ///replace the values of these variables with your database credentials
        ////////////////////////////////////////////////////////////////////////
        private string dbID = "sweet16";
        private string dbPass = "!!Sweet16";
        private string dbName = "sweet16";
        ////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////
        ///call this method anywhere that you need the connection string!
        ////////////////////////////////////////////////////////////////////////
        private string getConString()
        {
            return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName + "; UID=" + dbID + "; PASSWORD=" + dbPass;
        }
        ////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////////
        //don't forget to include this decoration above each method that you want
        //to be exposed as a web service!
        [WebMethod(EnableSession = true)]
        /////////////////////////////////////////////////////////////////////////
        public string TestConnection()
        {
            try
            {
                string testQuery = "select * from testQuery";

                ////////////////////////////////////////////////////////////////////////
                ///here's an example of using the getConString method!
                ////////////////////////////////////////////////////////////////////////
                MySqlConnection con = new MySqlConnection(getConString());
                ////////////////////////////////////////////////////////////////////////

                MySqlCommand cmd = new MySqlCommand(testQuery, con);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                return "Success";
            }
            catch (Exception e)
            {
                return "Something went wrong, please check your credentials and db name and try again.  Error: " + e.Message;
            }
        }



        //Create New Account Logic//
        [WebMethod(EnableSession = true)]
        public void RequestAccount(string fName, string lName, string userName, string password, string email,
                                    string year, string college, string campus)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["sweet16"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into Register (fname, lname, userName, password, email, year, college, campus) " +
                "values(@fnameValue, @lnameValue, @usernameValue, @passwordValue, @emailValue, @yearValue, @collegeValue, @campusValue); SELECT LAST_INSERT_ID();";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //sqlCommand.Parameters.AddWithValue("@idRegisterValue", HttpUtility.UrlDecode(idRegister));
            sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(fName));
            sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lName));
            sqlCommand.Parameters.AddWithValue("@usernameValue", HttpUtility.UrlDecode(userName));
            sqlCommand.Parameters.AddWithValue("@passwordValue", HttpUtility.UrlDecode(password));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlCommand.Parameters.AddWithValue("@yearValue", HttpUtility.UrlDecode(year));
            sqlCommand.Parameters.AddWithValue("@collegeValue", HttpUtility.UrlDecode(college));
            sqlCommand.Parameters.AddWithValue("@campusValue", HttpUtility.UrlDecode(campus));

            //this time, we're not using a data adapter to fill a data table.  We're just
            //opening the connection, telling our command to "executescalar" which says basically
            //execute the query and just hand me back the number the query returns (the ID, remember?).
            //don't forget to close the connection!
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                //here, you could use this accountID for additional queries regarding
                //the requested account.  Really this is just an example to show you
                //a query where you get the primary key of the inserted row back from
                //the database!
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }


        //Login Logic//
        [WebMethod(EnableSession = true)]
        public bool LogOn(string uid, string pass)
        {
            bool success = false;

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["sweet16"].ConnectionString;
            string sqlSelect = "SELECT idRegister FROM Register WHERE (email=@idValue and password=@passValue) or (userName=@idValue and password=@passValue);";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            DataTable sqlDt = new DataTable();
            sqlDa.Fill(sqlDt);

            if (sqlDt.Rows.Count > 0)
            {
                Session["id"] = sqlDt.Rows[0]["idRegister"];
                success = true;
            }

            return success;
        }

        //Log Off Method
        [WebMethod(EnableSession = true)]
        public bool LogOff()
        {
            //if they log off, then we remove the session.  That way, if they access
            //again later they have to log back on in order for their ID to be back
            //in the session!
            Session.Abandon();
            return true;
        }

        [WebMethod(EnableSession = true)]
        public string NewEvent(string className, string desc, string date, string time, string location)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["sweet16"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into events (className, descr, date, time, location) " +
                "values(@classNameValue, @descValue, @dateValue, @timeValue, @locationValue); SELECT LAST_INSERT_ID();";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //sqlCommand.Parameters.AddWithValue("@idRegisterValue", HttpUtility.UrlDecode(idRegister));
            sqlCommand.Parameters.AddWithValue("@classNameValue", HttpUtility.UrlDecode(className));
            sqlCommand.Parameters.AddWithValue("@descValue", HttpUtility.UrlDecode(desc));
            sqlCommand.Parameters.AddWithValue("@dateValue", HttpUtility.UrlDecode(date));
            sqlCommand.Parameters.AddWithValue("@timeValue", HttpUtility.UrlDecode(time));
            sqlCommand.Parameters.AddWithValue("@locationValue", HttpUtility.UrlDecode(location));
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                //here, you could use this accountID for additional queries regarding
                //the requested account.  Really this is just an example to show you
                //a query where you get the primary key of the inserted row back from
                //the database!
                sqlConnection.Close();
                return "success";
            }
            catch (Exception e)
            {
                return "error" + e.Message;
            }
            //sqlConnection.Close();
        }

        //getEventInfo
        [WebMethod(EnableSession = true)]
        public Event[] GetEvents()
        {

            //WE ONLY SHARE Events WITH LOGGED IN USERS!
            if (Session["id"] != null)
            {
                DataTable sqlDt = new DataTable("events");

                string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["sweet16"].ConnectionString;
                string sqlSelect = "select * from events where curdate() <= date order by date asc";

                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                //gonna use this to fill a data table
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                //filling the data table
                sqlDa.Fill(sqlDt);

                //loop through each row in the dataset, creating instances
                //of our container class Event.  Fill each eveny with
                //data from the rows, then dump them in a list.
                List<Event> events = new List<Event>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {

                    events.Add(new Event
                    {
                        eventId = Convert.ToInt32(sqlDt.Rows[i]["idevents"]),
                        className = sqlDt.Rows[i]["className"].ToString(),
                        description = sqlDt.Rows[i]["descr"].ToString(),
                        date = sqlDt.Rows[i]["date"].ToString(),
                        time = sqlDt.Rows[i]["time"].ToString(),
                        location = sqlDt.Rows[i]["location"].ToString()
                    });
                }
                //convert the list of events to an array and return!
                return events.ToArray();
            }
            else
            {
                //if they're not logged in, return an empty event
                return new Event[0];
            }
        }


        //getEventInfo
        [WebMethod(EnableSession = true)]
        public Profile[] PersonalInfo()
        {

            //WE ONLY SHARE Events WITH LOGGED IN USERS!
            if (Session["id"] != null)
            {
                DataTable sqlDt = new DataTable("Register");

                string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["sweet16"].ConnectionString;
                string sqlSelect = "select * from Register where ";

                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                //gonna use this to fill a data table
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                //filling the data table
                sqlDa.Fill(sqlDt);

                //loop through each row in the dataset, creating instances
                //of our container class Event.  Fill each eveny with
                //data from the rows, then dump them in a list.
                List<Profile> profile = new List<Profile>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {

                    profile.Add(new Profile
                    {
                        registerId = Convert.ToInt32(sqlDt.Rows[i]["idregister"]),
                        fName = sqlDt.Rows[i]["fName"].ToString(),
                        lName = sqlDt.Rows[i]["lName"].ToString(),
                        year = sqlDt.Rows[i]["date"].ToString(),
                        college = sqlDt.Rows[i]["time"].ToString(),
                        campus = sqlDt.Rows[i]["location"].ToString()
                    });
                }
                //convert the list of events to an array and return!
                return profile.ToArray();
            }
            else
            {
                //if they're not logged in, return an empty event
                return new Profile[0];
            }
        }
    }
}
 



