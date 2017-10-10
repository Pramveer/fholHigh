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
            DataTable dTable = phd.getQueryDataForChart("enrolledStatus", true,string.Empty);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dTable);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getRxTrendAndActivatedData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("rxTrendActivated", true, string.Empty);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getActivePatientsData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("activePatients", false, string.Empty);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientsListData()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientList", false, string.Empty);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientCompliance(string userID)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientComplianceChart", true, userID);
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
        public static string getPatientComplianceDrillDown(string userID,string pointtype)
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientComplianceChart", true, userID);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }

        [WebMethod]
        public static string getPatientComplianceComparative()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("comparitiveBaselineChart", true, string.Empty);
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
        public static string getPatientComplianceComparativeDrillDown()
        {
            PhysicianDashBoard phd = new PhysicianDashBoard();
            DataTable data = phd.getQueryDataForChart("patientComplianceChart", true, string.Empty);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);

            return jsonString.ToString();
        }


        public DataTable getQueryDataForChart(string chartName, bool isProcedure,string  userID)
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
                    //   query = "sp_getDataForEnrolledStatusChart";
                    break;

                case "activePatients":
                    //   query = "SELECT DATEPART(MONTH, patient.OperationDate) as month , COUNT(1) as pCount FROM _Patient as patient JOIN _PatientStatusType as status ON patient.PatientStatusID = status.PatientStatusID where patient.PatientStatusID = 1 and DATEPART(YEAR, patient.OperationDate) = 2017 GROUP BY DATEPART(MONTH, patient.OperationDate) ORDER BY DATEPART(MONTH, patient.OperationDate)";
                    break;

                case "rxTrendActivated":
                    //  query = "sp_getDataForRxAndNewActivated";
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
    }

}