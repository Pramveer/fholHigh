using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace FHOL
{
    public partial class PhysicianDashBoard : System.Web.UI.Page
    {
        string strcon = string.Empty;
        SqlConnection DbConnection = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        [WebMethod]
        public static string getEnrolledPatientStatusData()
        {

            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable dTable =  phd.getQueryDataForChart("enrolledStatus", true);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dTable);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getRxTrendAndActivatedData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("rxTrendActivated", true);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getActivePatientsData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("activePatients", false);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientsListData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientList", false);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        public DataTable getQueryDataForChart(string chartName, bool isProcedure)
        {
            strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;
            string query = string.Empty;

            // prepare the query for the chart
            switch (chartName)
            {
                case "enrolledStatus":
                    query = "sp_getDataForEnrolledStatusChart";
                    break;

                case "activePatients":
                    query = "SELECT DATEPART(MONTH, patient.OperationDate) as month , COUNT(1) as pCount FROM _Patient as patient JOIN _PatientStatusType as status ON patient.PatientStatusID = status.PatientStatusID where patient.PatientStatusID = 1 and DATEPART(YEAR, patient.OperationDate) = 2017 GROUP BY DATEPART(MONTH, patient.OperationDate) ORDER BY DATEPART(MONTH, patient.OperationDate)";
                    break;

                case "rxTrendActivated":
                    query = "sp_getDataForRxAndNewActivated";
                    break;

                case "patientList":
                    query = "SELECT TOP 100 p.PatientID , p.DateOfBirth, u.FirstName, u.LastName, u.MiddleName  FROM [EmbeddedIndia].[dbo].[_Patient] p  join ._User u on u.UserID = p.UserID   AND u.CRMID IS NOT NULL   AND DATEPART(YEAR,p.OperationDate) = 2017  and p.Active = 1;";
                    break;

                default:
                    break;
            }

            // create connection and get the data
            DbConnection = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand(query, DbConnection);

            if(isProcedure)
            {
                cmd.CommandType = CommandType.StoredProcedure;
            }

            DbConnection.Open();
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            DbConnection.Close();

            return dt;

        }
    }

}