using CPTest.Connections;
using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IAppointmentSqlServices
    { 
        public int CreateAppointment(DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            int iLinkedRef, int mpi, string appType, int duration, string sStaffCode, string sInstructions);
        public void ModifyAppointment(int refID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sStaffCode, string sInstructions, string sCancellation, int famMPI, bool isReturnToWL);        
    }
    public class AppointmentSqlServices : IAppointmentSqlServices
{
        private readonly IConfiguration _config;
        public AppointmentSqlServices(IConfiguration config) 
        {
            _config = config;
        }        
        public int CreateAppointment(DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation, 
            int iLinkedRef, int mpi, string appType, int duration, string sStaffCode, string sInstructions)
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
            cmd.Parameters.Add("@MPI", SqlDbType.BigInt).Value = mpi;
            cmd.Parameters.Add("@ApptType", SqlDbType.VarChar).Value = appType;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@UserStaffCode", SqlDbType.VarChar).Value = sStaffCode;            
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            var returnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int); //return success or not
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            var iReturnValue = (int)returnValue.Value;
            con.Close();

            return iReturnValue;
        }
        public void ModifyAppointment(int refID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sStaffCode, string sInstructions, string sCancellation, int famMPI, bool isReturnToWL)
        {
            DateTime dTim = DateTime.Parse("1899-12-30 " + appTime);

            if (appWith2 == null) { appWith2 = ""; }
            if (appWith3 == null) { appWith3 = ""; }
            if (sInstructions == null) { sInstructions = ""; }
            if (sCancellation == null) { sCancellation = ""; }
            int returnToWL = 0;
            if (isReturnToWL) { returnToWL = 1; }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerModifyAppointment]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RefID", SqlDbType.Int).Value = refID;
            cmd.Parameters.Add("@ApptDate", SqlDbType.DateTime).Value = appDate;
            cmd.Parameters.Add("@ApptTime", SqlDbType.DateTime).Value = dTim;
            cmd.Parameters.Add("@ApptWith1", SqlDbType.VarChar).Value = appWith1;
            cmd.Parameters.Add("@ApptWith2", SqlDbType.VarChar).Value = appWith2;
            cmd.Parameters.Add("@ApptWith3", SqlDbType.VarChar).Value = appWith3;
            cmd.Parameters.Add("@ApptLocation", SqlDbType.VarChar).Value = appLocation;
            cmd.Parameters.Add("@ApptType", SqlDbType.VarChar).Value = appType;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@UserStaffCode", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@MachineName", SqlDbType.VarChar).Value = System.Environment.MachineName;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.Parameters.Add("@cancellation", SqlDbType.VarChar).Value = sCancellation;
            cmd.Parameters.Add("@famMPI", SqlDbType.Int).Value = famMPI;
            cmd.Parameters.Add("@returnToWL", SqlDbType.Int).Value = returnToWL;
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
