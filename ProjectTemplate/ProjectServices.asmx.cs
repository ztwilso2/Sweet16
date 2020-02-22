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
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
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
				return "Something went wrong, please check your credentials and db name and try again.  Error: "+e.Message;
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
            string sqlSelect = "SELECT idRegister FROM Register WHERE email=@idValue and password=@passValue";

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
        public void NewEvent(string className, string desc, string date, string time, string location)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["sweet16"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into events (className, desc, date, time, location) " +
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
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }
    }
}
 



