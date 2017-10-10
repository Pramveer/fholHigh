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
            DataTable data = phd.getQueryDataForChart("patientList", true, string.Empty, false);

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

        public string prepareSqlQueryForPatients()
        {
            string query = "";

           
            return query;
        }
    }

}