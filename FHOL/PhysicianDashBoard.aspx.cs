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
using System.Text.RegularExpressions;

namespace FHOL
{
    public partial class PhysicianDashBoard : System.Web.UI.Page
    {
        string strcon = string.Empty;
        SqlConnection DbConnection = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            UserName.Value = Context.User.Identity.Name;
        }

        [WebMethod]
        public static string getEnrolledPatientStatusData(string dataParams)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable dTable = phd.getQueryDataForChart("enrolledStatus", true, dataParams, true);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dTable);

            return jsonString.ToString();

        }


        [WebMethod]
        public static int getUserID(string user)
        {
            int userid = 0;
           string strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;
            string sqlstringalert = "SELECT UserID from _User where UserName ='" + user + "';";
            SqlConnection DbConnection = new SqlConnection(strcon);
            DbConnection.Open();
            SqlCommand comm = new SqlCommand(sqlstringalert, DbConnection);
            comm.CommandTimeout = 0;
            userid = Convert.ToInt16(comm.ExecuteScalar().ToString());
            //string jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(userid);
            DbConnection.Close();
            return userid;
        }

        [WebMethod]
        public static string getRxTrendAndActivatedData(string dataParams)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("rxTrendActivated", true, dataParams, true);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getActivePatientsData(string dataParams)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("activePatients", true, dataParams, true);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientsListData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientList", false, string.Empty, false);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientCompliance(string userID)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientComplianceChart", true, userID, false);
            DataRow[] resultmorethan8 = data.Select("testnum >= 8");
            DataRow[] resultlessthan8 = data.Select("testnum < 8");
            int morethan8 = 0, lessthan8 = 0;
            Double permorethan8 = 0, perlessthan8 = 0;

            morethan8 = resultmorethan8.Length;
            lessthan8 = resultlessthan8.Length;
            if (morethan8 > 0)
                permorethan8 = Math.Round(((Double)morethan8 / (data.Rows.Count)) * 100, 2); // Math.Round(((Double)morethan8 / (morethan8 + lessthan8)) * 100, 2);
            if (lessthan8 > 0)
                perlessthan8 = Math.Round(((Double)lessthan8 / (data.Rows.Count)) * 100, 2);

            List<PieSeriesData> pieData = new List<PieSeriesData>();
            pieData.Add(new PieSeriesData { Name = ">=8", Y = permorethan8, yvalue = morethan8, Color = "#5b9bd5" });
            pieData.Add(new PieSeriesData { Name = "<8", Y = perlessthan8, yvalue = lessthan8, Color = "#70ad47" });

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(pieData);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientComplianceDrillDown(string userID, string pointtype)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientComplianceChart", true, userID, false);
            DataRow[] results = null;
            if (pointtype == "<8")
                results = data.Select("testnum < 8");
            else if (pointtype == ">=8")
                results = data.Select("testnum >= 8");
            DataTable dt = data.Clone();
            foreach (DataRow row in results)
            {
                dt.ImportRow(row);
            }

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dt);

            return jsonString.ToString();
        }


        [WebMethod]
        public static string getProviderAlerts(string userID)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("openAlertsValue", false, userID, false);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data.Rows.Count);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getProviderAlertsDrillDown(string userID)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("openAlertsValue", false, userID, false);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientComplianceComparative()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("comparitiveBaselineChart", true, string.Empty, false);
            DataRow[] resultmorethan8 = data.Select("testnum >= 8");
            DataRow[] resultlessthan8 = data.Select("testnum < 8");
            int morethan8 = 0, lessthan8 = 0;
            Double permorethan8 = 0, perlessthan8 = 0;

            morethan8 = resultmorethan8.Length;
            lessthan8 = resultlessthan8.Length;
            if (morethan8 > 0)
                permorethan8 = Math.Round(((Double)morethan8 / (data.Rows.Count)) * 100, 2); // Math.Round(((Double)morethan8 / (morethan8 + lessthan8)) * 100, 2);
            if (lessthan8 > 0)
                perlessthan8 = Math.Round(((Double)lessthan8 / (data.Rows.Count)) * 100, 2);

            List<PieSeriesData> pieData = new List<PieSeriesData>();
            pieData.Add(new PieSeriesData { Name = ">=8", Y = permorethan8, yvalue = morethan8, Color = "#5b9bd5" });
            pieData.Add(new PieSeriesData { Name = "<8", Y = perlessthan8, yvalue = lessthan8, Color = "#70ad47" });

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(pieData);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientComplianceComparativeDrillDown(string pointtype)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();

            DataTable data = phd.getQueryDataForChart("patientComplianceChart", false, string.Empty, false);
            DataRow[] results = null;
            if (pointtype == "<8")
                results = data.Select("testnum < 8");
            else if (pointtype == ">=8")
                results = data.Select("testnum >= 8");
            DataTable dt = data.Clone();
            foreach (DataRow row in results)
            {
                dt.ImportRow(row);
            }

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dt);

            return jsonString.ToString();
        }


        public DataTable getQueryDataForChart(string chartName, bool isProcedure, string  paramData, bool isMultipleParams)
        {
            strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;
            string query = string.Empty;

            string userID = string.Empty;
            DateTime minDate = new DateTime();
            DateTime maxDate = new DateTime();

            if (isMultipleParams)
            {
                string[] paramArray = Regex.Split(paramData, "##");
                userID = paramArray[0];
                minDate = Convert.ToDateTime(paramArray[1]);
                maxDate = Convert.ToDateTime(paramArray[2]);
            }
            else
            {
                userID = paramData;
            }

            // prepare the query for the chart
            switch (chartName)
            {
                case "enrolledStatus":
                    query = "sp_getDataForEnrolledStatusChart";
                    break;

                case "activePatients":
                    query = "sp_getDataForCommulativeActivation";
                    break;

                case "rxTrendActivated":
                    query = "sp_getDataForRxAndNewActivated";
                    break;

                case "patientList":
                    query = "SELECT TOP 100 p.PatientID , p.DateOfBirth, u.FirstName, u.LastName, u.MiddleName  FROM [EmbeddedIndia].[dbo].[_Patient] p  join ._User u on u.UserID = p.UserID   AND u.CRMID IS NOT NULL   AND DATEPART(YEAR,p.OperationDate) = 2017  and p.Active = 1;";
                    break;

                case "openAlertsValue":
                    query = "select * from ProviderDashboard_OpenAlert where PrescribingECPID = " + userID;
                    break;

                case "patientComplianceChart":
                    query = "sp_ProviderDashboard_PatientComplianceChartData";
                    break;

                case "comparitiveBaselineChart":
                    query = "sp_ProviderDashboard_PatientComplianceChartData";
                    break;
                default:
                    break;
            }

            // create connection and get the data
            DbConnection = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand(query, DbConnection);
            cmd.CommandTimeout = 0;

            if (isProcedure)
            {
                cmd.CommandType = CommandType.StoredProcedure;
            }

            // For Parameter Passing 
            switch (chartName)
            {
                case "enrolledStatus":
                    cmd.Parameters.AddWithValue("@PrescribingECPID", userID);
                    cmd.Parameters.AddWithValue("@MinDate", minDate);
                    cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                    break;

                case "activePatients":
                    cmd.Parameters.AddWithValue("@PrescribingECPID", userID);
                    cmd.Parameters.AddWithValue("@MinDate", minDate);
                    cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                    break;

                case "rxTrendActivated":
                    cmd.Parameters.AddWithValue("@PrescribingECPID", userID);
                    cmd.Parameters.AddWithValue("@MinDate", minDate);
                    cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                    break;

                case "patientList":
                    //   query = "SELECT TOP 100 p.PatientID , p.DateOfBirth, u.FirstName, u.LastName, u.MiddleName  FROM [EmbeddedIndia].[dbo].[_Patient] p  join ._User u on u.UserID = p.UserID   AND u.CRMID IS NOT NULL   AND DATEPART(YEAR,p.OperationDate) = 2017  and p.Active = 1;";
                    break;

                case "patientComplianceChart":
                    cmd.Parameters.AddWithValue("@PrescribingECPID", userID);
                    break;
                case "comparitiveBaselineChart":
                    cmd.Parameters.AddWithValue("@PrescribingECPID", 0);
                    break;
                default:
                    break;
            }

            DbConnection.Open();
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            DbConnection.Close();

            return dt;

        }


        [WebMethod]
        public static string getPatientListForEnrolledChart(string dataParams)
        {
            string userID = string.Empty;
            string pointName = string.Empty;
            DateTime minDate = new DateTime();
            DateTime maxDate = new DateTime();
            string preparedQuery = string.Empty;

            string[] paramArray = Regex.Split(dataParams, "##");
            userID = paramArray[0];
            minDate = Convert.ToDateTime(paramArray[1]);
            maxDate = Convert.ToDateTime(paramArray[2]);
            pointName = paramArray[3];
            
            PhysicianDashBoard phd = new PhysicianDashBoard();

            preparedQuery = phd.getBasicQueryForPatientsList(userID, minDate, maxDate);

            if(pointName == "Never Tested")
            {
                preparedQuery += " AND Datedeviceshipped is not null and Devicetransmittingdate is null and BaselineEstDate is null";
            }
            else if(pointName == "Baseline Progress")
            {
                preparedQuery += " AND Datedeviceshipped is not null  and Devicetransmittingdate is not null and BaselineEstDate is null";
            }
            else if (pointName == "Active Patients")
            {
                preparedQuery += " AND BaselineEstDate is not null";
            }
            else if(pointName == "CEBL")
            {
                preparedQuery += " AND PatientStatusID = 6";
            }

            DataTable data = phd.getPatientsListForCharts(preparedQuery);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientListForRxAndActive(string dataParams)
        {
            string userID = string.Empty;
            string seriesName = string.Empty;
            string monthId = string.Empty;
            DateTime minDate = new DateTime();
            DateTime maxDate = new DateTime();
            string preparedQuery = string.Empty;

            string[] paramArray = Regex.Split(dataParams, "##");
            userID = paramArray[0];
            minDate = Convert.ToDateTime(paramArray[1]);
            maxDate = Convert.ToDateTime(paramArray[2]);
            seriesName = paramArray[3];
            monthId = paramArray[4];

            PhysicianDashBoard phd = new PhysicianDashBoard();

            preparedQuery = phd.getBasicQueryForPatientsList(userID, minDate, maxDate);

            if (seriesName == "Active")
            {
                preparedQuery += " AND BaselineEstDate is not null and  PatientStatusID = 1  and (LeftEye in ('IDTF', 'Production')  or RightEye in ('IDTF', 'Production') )";
            }
            else
            {
                preparedQuery += " ";
            }

            if(monthId != "NA")
            {
                preparedQuery += " AND DATEPART(MONTH, CreatedOn) = " + monthId;
            }

            DataTable data = phd.getPatientsListForCharts(preparedQuery);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }
        
        [WebMethod]
        public static string getPatientListForCummilative(string dataParams)
        {
            string userID = string.Empty;
            string monthId = string.Empty;
            DateTime minDate = new DateTime();
            DateTime maxDate = new DateTime();
            string preparedQuery = string.Empty;

            string[] paramArray = Regex.Split(dataParams, "##");
            userID = paramArray[0];
            minDate = Convert.ToDateTime(paramArray[1]);
            maxDate = Convert.ToDateTime(paramArray[2]);
            monthId = paramArray[3];

            PhysicianDashBoard phd = new PhysicianDashBoard();

            preparedQuery = phd.getBasicQueryForPatientsList(userID, minDate, maxDate);

            if (monthId != "NA")
            {
                preparedQuery += " AND DATEPART(MONTH, CreatedOn) = " + monthId;
            }

            DataTable data = phd.getPatientsListForCharts(preparedQuery);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();

        }

        DataTable getPatientsListForCharts(string query)
        {
            strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;

            DbConnection = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand(query, DbConnection);
            cmd.CommandTimeout = 0;

            DbConnection.Open();
            cmd.ExecuteNonQuery();

            DataTable dtable = new DataTable();
            dtable.Load(cmd.ExecuteReader());

            DbConnection.Close();

            return dtable;
        }

        string getBasicQueryForPatientsList(string userID, DateTime minDate, DateTime maxDate)
        {
            string query = "";

            query = "SELECT PatientID , DateOfBirth, FirstName, LastName FROM ContactBase WHERE Active = 1 AND PrescribingECPID = " + userID;
            query += " AND CreatedOn BETWEEN '" + minDate + "' AND '" + maxDate + "'";

            return query;
        }
    }

}