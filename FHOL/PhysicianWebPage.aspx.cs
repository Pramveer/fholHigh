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
using System.Drawing;


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
                            DataTable patientcompliance = ReturnPatientCompliance_DataTable(DbConnection, Convert.ToInt16(userId));                           
                             BindPatientCompliance(patientcompliance);
                            DataTable Comparativepatientcompliance = ReturnPatientCompliance_DataTable(DbConnection, 0);
                            comparativeBindPatientCompliance(Comparativepatientcompliance);
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

        private void BindPatientCompliance( DataTable patientcompliance)
        {
DataRow[] resultmorethan8 = patientcompliance.Select("testnum >= 8");
                            DataRow[] resultlessthan8 = patientcompliance.Select("testnum < 8");
                            int morethan8 = 0, lessthan8 = 0;
                            Double permorethan8 = 0, perlessthan8 = 0;
                            //foreach (DataRow row in resultmorethan8)
                            //{
                            morethan8 = resultmorethan8.Length;// morethan8 + Convert.ToInt16(row["testnum"].ToString());
                            //}
                            //foreach (DataRow row in resultlessthan8)
                            //{
                            lessthan8 = resultlessthan8.Length;// lessthan8 + Convert.ToInt16(row["testnum"].ToString());
                            //}
                            if (morethan8 > 0)
                                permorethan8 = Math.Round(((Double)morethan8 / (patientcompliance.Rows.Count)) * 100, 2); // Math.Round(((Double)morethan8 / (morethan8 + lessthan8)) * 100, 2);
                            if (lessthan8 > 0)
                                perlessthan8 = Math.Round(((Double)lessthan8 / (patientcompliance.Rows.Count)) * 100, 2);

                            List<PieSeriesData> pieData = new List<PieSeriesData>();
                            pieData.Add(new PieSeriesData { Name = " >=8 ", Y = permorethan8, yvalue = morethan8 });
                            pieData.Add(new PieSeriesData { Name = " <8 ", Y = perlessthan8, yvalue = lessthan8 });
                                               

                            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
                            .SetTitle(new Title { Text = "" })
                            .SetCredits(new Credits { Enabled = false})
                     .SetTooltip(new Tooltip { Formatter = "function() { return '<b>Counts = </b>'+ this.point.yvalue + '<br/> <b>% = </b>' +this.percentage +' %'; }" })
                                .SetPlotOptions(new PlotOptions
                                {
                                    Pie = new PlotOptionsPie
                                    {
                                        AllowPointSelect = true,
                                        Cursor = Cursors.Pointer,
                                        DataLabels = new PlotOptionsPieDataLabels
                                        {
                                            Color = ColorTranslator.FromHtml("#000000"),
                                            ConnectorColor = ColorTranslator.FromHtml("#000000"),
                                            Formatter = "function() { return ''+ this.percentage +' %'; }"
                                        },
                                        ShowInLegend = true
                                    }
                                   
                                })
                                
                    .SetSeries(new[] { 
                new Series
                {
                  Type = ChartTypes.Pie,     Data = new Data(pieData.ToArray())
                }});

                            ltrChart.Text = chart.ToHtmlString();

        }

        private void comparativeBindPatientCompliance(DataTable patientcompliance)
        {
            DataRow[] resultmorethan8 = patientcompliance.Select("testnum >= 8");
            DataRow[] resultlessthan8 = patientcompliance.Select("testnum < 8");
            int morethan8 = 0, lessthan8 = 0;
            Double permorethan8 = 0, perlessthan8 = 0;
            //foreach (DataRow row in resultmorethan8)
            //{
            morethan8 = resultmorethan8.Length;// morethan8 + Convert.ToInt16(row["testnum"].ToString());
            //}
            //foreach (DataRow row in resultlessthan8)
            //{
            lessthan8 = resultlessthan8.Length;// lessthan8 + Convert.ToInt16(row["testnum"].ToString());
            //}
            if (morethan8 > 0)
                permorethan8 = Math.Round(((Double)morethan8 / (patientcompliance.Rows.Count)) * 100, 2); // Math.Round(((Double)morethan8 / (morethan8 + lessthan8)) * 100, 2);
            if (lessthan8 > 0)
                perlessthan8 = Math.Round(((Double)lessthan8 / (patientcompliance.Rows.Count)) * 100, 2);

            List<PieSeriesData> pieData = new List<PieSeriesData>();
            pieData.Add(new PieSeriesData { Name = " >=8 ", Y = permorethan8, yvalue = morethan8 });
            pieData.Add(new PieSeriesData { Name = " <8 ", Y = perlessthan8, yvalue = lessthan8 });


            DotNet.Highcharts.Highcharts chartt = new DotNet.Highcharts.Highcharts("chartt")
            .SetTitle(new Title { Text = "" })
            .SetCredits(new Credits { Enabled = false })
            
     .SetTooltip(new Tooltip { Formatter = "function() { return '<b>Counts = </b>'+ this.point.yvalue + '<br/> <b>% = </b>' +this.percentage +' %'; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        Colors = { "#000000", "#cccccc" },
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Color = ColorTranslator.FromHtml("#000000"),
                            ConnectorColor = ColorTranslator.FromHtml("#000000"),
                            Formatter = "function() { return ''+ this.percentage +' %'; }"
                        },
                        ShowInLegend = true
                    }
                })

    .SetSeries(new[] { 
                new Series
                {
                  Type = ChartTypes.Pie,     Data = new Data(pieData.ToArray())
                }});


            ltrComparativeChart.Text = chartt.ToHtmlString();

        }

        private DataTable ReturnPatientCompliance_DataTable(SqlConnection dbconn, int userid)
        {
            string sqlstringalert = "sp_ProviderDashboard_PatientComplianceChartData";
            SqlCommand comm = new SqlCommand(sqlstringalert, dbconn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@PrescribingECPID", userid);
            dbconn.Open();
            comm.ExecuteNonQuery();
            DataTable dt_patientcompliance = new DataTable();
            dt_patientcompliance.Load(comm.ExecuteReader());
            dbconn.Close();
            return dt_patientcompliance;
        }


    }
}