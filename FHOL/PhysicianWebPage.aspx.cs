using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNet.Highcharts;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using System.Globalization;

namespace FHOL
{
    public partial class PhysicianWebPage : System.Web.UI.Page
    {
        string strcon = string.Empty;
        SqlConnection DbConnection = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;
            DbConnection = new SqlConnection(strcon);
            RenderActivePatientsData(DbConnection);
            RenderEnrolledPatientStatus(DbConnection);
            RenderRxTrendActivatedChart(DbConnection);

            if (Membership.ValidateUser("Nisha", "Nisha123!"))
            {

                if (!IsPostBack)
                {
                    try
                    {
                        if (DbConnection.State == ConnectionState.Closed)
                        {
                            string userId = ReturnUserID(DbConnection, "Nisha");
                            DataTable dt = ReturnOpenAlerts_DataTable(DbConnection, userId);
                            lblopenalert.Text = dt.Rows.Count.ToString();

                            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
                            .SetXAxis(new XAxis
                            {
                                Categories = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
                            })
                            .SetSeries(new[] { 
                            new Series
                            {
                              Type = ChartTypes.Pie,  Data = new Data(new object[] { 19.9, 21.5, 86.4, 99.2, 144.0, 176.0, 635.6, 648.5, 346.4, 121.1, 95.6, 100.4 })
                            }});

                            ltrChart.Text = chart.ToHtmlString();

                            
                        }
                    }
                    catch { }
                }
            }
        }

        private string ReturnUserID(SqlConnection dbconn, string username)
        {
            string sqlstringalert = "SELECT UserID from _User where UserName ='" + username + "';";
            dbconn.Open();
            SqlCommand comm = new SqlCommand(sqlstringalert, dbconn);
            var userId = "4222";// comm.ExecuteScalar().ToString();
            dbconn.Close();
            return userId.ToString();
        }

        private DataTable ReturnOpenAlerts_DataTable(SqlConnection dbconn, string userid)
        {
            string sqlstringalert = "SELECT * from ProviderDashboard_OpenAlert where PrescribingECPID=" + userid + ";";
            dbconn.Open();
            SqlCommand comm = new SqlCommand(sqlstringalert, dbconn);
            DataTable dt_openalerts = new DataTable();
            dt_openalerts.Load(comm.ExecuteReader());
            dbconn.Close();
            return dt_openalerts;
        }

        protected void lblopenalert_Click(object sender, EventArgs e)
        {
            BindGrid();
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup();", true);
        }

        private void BindGrid()
        {
            string userId = ReturnUserID(DbConnection, "Nisha");
            DataTable dt = ReturnOpenAlerts_DataTable(DbConnection, userId);
            grdAlerts.DataSource = dt;
            grdAlerts.DataBind();
        }


        private void RenderActivePatientsData(SqlConnection dbConn)
        {
            string query = "SELECT DATEPART(MONTH, patient.OperationDate) as month , COUNT(1) as pCount FROM _Patient as patient JOIN _PatientStatusType as status ON patient.PatientStatusID = status.PatientStatusID where patient.PatientStatusID = 1 and DATEPART(YEAR, patient.OperationDate) = 2017 GROUP BY DATEPART(MONTH, patient.OperationDate) ORDER BY DATEPART(MONTH, patient.OperationDate)";
            dbConn.Open();
            SqlCommand cmd = new SqlCommand(query, dbConn);

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            dbConn.Close();
            // string jsonData = JsonConvert.SerializeObject(dt, Formatting.Indented);
            // ActivePatientsChart.Text = jsonData;

            string[] categories = new string[dt.Rows.Count];
            var categoryData = new List<object[]>();
            int counter = 0;

            foreach (DataRow row in dt.Rows)
            {
                //TextBox1.Text = row["ImagePath"].ToString();
                categories[counter] = GetMonthNameByNumber(Convert.ToInt32(row["month"].ToString()));
                categoryData.Add(new object[] { row["pCount"]});

                counter ++;

            }

            Highcharts pChart = new Highcharts("pActiveChart");
            pChart.SetXAxis(new XAxis {
                Categories = categories
            });

            pChart.SetSeries(new Series
            {
                Type = ChartTypes.Line,
                Name= "Active",
                Data = new Data(categoryData.ToArray())
            });

            ActivePatientsChart.Text = pChart.ToHtmlString();

        }

        private void RenderEnrolledPatientStatus(SqlConnection dbConn)
        {

            string query = "SELECT status.Name, COUNT(1) as pCount FROM _Patient as patient JOIN _PatientStatusType as status ON patient.PatientStatusID = status.PatientStatusID WHERE DATEPART(YEAR, patient.OperationDate) = 2017 GROUP BY status.Name";
            dbConn.Open();

            SqlCommand cmd = new SqlCommand(query, dbConn);

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            dbConn.Close();

            string[] categories = new string[dt.Rows.Count];
            var categoryData = new List<object[]>();
            int counter = 0;

            foreach (DataRow row in dt.Rows)
            {
                //TextBox1.Text = row["ImagePath"].ToString();
                categories[counter] = row["Name"].ToString();
                categoryData.Add(new object[] { row["pCount"] });

                counter++;

            }

            Highcharts eChart = new Highcharts("enrolledChart");
            eChart.SetXAxis(new XAxis
            {
                Categories = categories
            });

            eChart.SetSeries(new Series
            {
                Type = ChartTypes.Pie,
                Data = new Data(categoryData.ToArray())
            });

            eChart.SetPlotOptions(new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    DataLabels = new PlotOptionsPieDataLabels
                    {
                        Formatter = "function() { return ''+ this.y; }"
                    }
                }
            });

            EnrolledPatientStatusChart.Text = eChart.ToHtmlString();
        }

        private void RenderRxTrendActivatedChart(SqlConnection dbConn)
        {

            string query = "SELECT DATEPART(MONTH, patient.OperationDate) as month , COUNT(1) as pCount, SUM(patient.PatientID)/ 100000 as testCount FROM _Patient as patient JOIN _PatientStatusType as status ON patient.PatientStatusID = status.PatientStatusID where patient.PatientStatusID = 1 and DATEPART(YEAR, patient.OperationDate) = 2017 GROUP BY DATEPART(MONTH, patient.OperationDate) ORDER BY DATEPART(MONTH, patient.OperationDate)";
            dbConn.Open();

            SqlCommand cmd = new SqlCommand(query, dbConn);

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            dbConn.Close();

            string[] categories = new string[dt.Rows.Count];
            var pCount = new List<object[]>();
            var testCount = new List<object[]>();
            int counter = 0;

            foreach (DataRow row in dt.Rows)
            {
                //TextBox1.Text = row["ImagePath"].ToString();
                categories[counter] = GetMonthNameByNumber(Convert.ToInt32(row["month"].ToString()));
                pCount.Add(new object[] { row["pCount"] });
                testCount.Add(new object[] { row["testCount"] });

                counter++;

            }

            Highcharts eChart = new Highcharts("rxActivityChart");
            eChart.SetXAxis(new XAxis
            {
                Categories = categories
            });

            eChart.SetSeries(new[]
            {
                new Series {
                    Type = ChartTypes.Line,
                    Name = "Rx",
                    Data = new Data(pCount.ToArray())
                },
                new Series
                {
                    Type = ChartTypes.Line,
                    Name = "Active",
                    Data = new Data(testCount.ToArray())
                }
            });
            

            RxTrendActivatedPatient.Text = eChart.ToHtmlString();
        }


        private string GetMonthNameByNumber(int monthNum)
        {
            string name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthNum);

            return name;
        }
    }
}