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

namespace FHOL
{
    public partial class PhysicianWebPage : System.Web.UI.Page
    {
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Response.Write("tet");
             if (Membership.ValidateUser("Nisha","Nisha123!"))
             {
                           
            if (!IsPostBack)
            {
                string strcon = ConfigurationManager.ConnectionStrings["DBEmbeddedIndiaConnection"].ConnectionString;
                SqlConnection DbConnection = new SqlConnection(strcon);
                try
                {
                    if (DbConnection.State == ConnectionState.Closed)
                    {
                        DbConnection.Open();
                        String sqlstringalert = "SELECT 1 as AlertID,_mf_alert_PosUr.*,convert(varchar(10),_Patient.DateOfBirth,101) as DOB,FirstName + ' '+LastName as PatientName,case when convert(varchar(10),AlertDate,101) = convert(varchar(10),GETDATE(),101) then 'Today' else convert(varchar(10),AlertDate,101) end as AlertDatet " +
                        ",dbo.fn_GetClinicName(_Patient.OfficeID) as Clinic, dbo.fn_GetPhysicianName(_Patient.PrescribingECPID) as Physician FROM _mf_alert_PosUr inner join _Patient on _mf_alert_PosUr.PatientID = _Patient.PatientID WHERE Reviewed = 0";
                        SqlCommand comm = new SqlCommand(sqlstringalert, DbConnection);

                        dt = new DataTable();
                        dt.Load(comm.ExecuteReader());
                        BindGrid();
                        lblopenalert.Text = dt.Rows.Count.ToString();
                        DbConnection.Close();
                    }
                    Response.Write("Connection successfull");
                }
                catch { }
            }
             } 
        }

        protected void lblopenalert_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            grdAlerts.DataSource = dt;
            grdAlerts.DataBind();
        }

    }
}