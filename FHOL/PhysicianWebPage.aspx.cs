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

    }
}