using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    public class SqlServices
    {
        private readonly IConfiguration _config;
        public SqlServices(IConfiguration config) 
        {
            _config = config;
        }
        private string strSQL;

        public void CreateAppointment(DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation, 
            int iLinkedRef, int iMPI, string appType, int iDuration, string sUsername, string sInstructions)
        {
            DateTime dTim = DateTime.Parse("1899-12-30 " + appTime);

            if (appWith2 == null) { appWith2 = ""; }
            if (appWith3 == null) { appWith3 = ""; }
            if (sInstructions == null) { sInstructions = ""; }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));            
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerCreateAppointment]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ApptDate", SqlDbType.DateTime).Value = appDate;
            cmd.Parameters.Add("@ApptTime", SqlDbType.DateTime).Value = dTim;
            cmd.Parameters.Add("@ApptWith1", SqlDbType.VarChar).Value = appWith1;
            cmd.Parameters.Add("@ApptWith2", SqlDbType.VarChar).Value = appWith2;
            cmd.Parameters.Add("@ApptWith3", SqlDbType.VarChar).Value = appWith3;
            cmd.Parameters.Add("@ApptLocation", SqlDbType.VarChar).Value = appLocation;
            cmd.Parameters.Add("@LinkedRefID", SqlDbType.BigInt).Value = iLinkedRef;
            cmd.Parameters.Add("@MPI", SqlDbType.BigInt).Value = iMPI;
            cmd.Parameters.Add("@ApptType", SqlDbType.VarChar).Value = appType;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = iDuration;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sUsername;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ModifyAppointment(int iRefID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int iDuration, string sUsername, string sInstructions, string sCancellation)
        {
            DateTime dTim = DateTime.Parse("1899-12-30 " + appTime);

            if (appWith2 == null) { appWith2 = ""; }
            if (appWith3 == null) { appWith3 = ""; }
            if (sInstructions == null) { sInstructions = ""; }
            if (sCancellation == null) { sCancellation = ""; }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerModifyAppointment]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RefID", SqlDbType.Int).Value = iRefID;
            cmd.Parameters.Add("@ApptDate", SqlDbType.DateTime).Value = appDate;
            cmd.Parameters.Add("@ApptTime", SqlDbType.DateTime).Value = dTim;
            cmd.Parameters.Add("@ApptWith1", SqlDbType.VarChar).Value = appWith1;
            cmd.Parameters.Add("@ApptWith2", SqlDbType.VarChar).Value = appWith2;
            cmd.Parameters.Add("@ApptWith3", SqlDbType.VarChar).Value = appWith3;
            cmd.Parameters.Add("@ApptLocation", SqlDbType.VarChar).Value = appLocation;            
            cmd.Parameters.Add("@ApptType", SqlDbType.VarChar).Value = appType;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = iDuration;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sUsername;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.Parameters.Add("@cancellation", SqlDbType.VarChar).Value = sCancellation;
            cmd.ExecuteNonQuery();
            con.Close();
        }


    }
}
