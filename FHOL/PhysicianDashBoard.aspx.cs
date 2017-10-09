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
            DataTable data =  phd.getQueryDataForChart();

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(data);
            return jsonString;
        }

        public DataTable getQueryDataForChart()
        {
            strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;
            string query = "sp_getDataForEnrolledStatusChart";
            DbConnection = new SqlConnection(strcon);
            SqlCommand cmd = new SqlCommand(query, DbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            DbConnection.Open();
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());

            DbConnection.Close();

            return dt;

        }
    }
}