using CPTest.Connections;
using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IAppointmentSqlServices
    { 
        public int CreateAppointment(DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            int iLinkedRef, int mpi, string appType, int duration, string sStaffCode, string sInstructions, int wlid, int slotID);
        public void ModifyAppointment(int refID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sStaffCode, string sInstructions, string sCancellation, int famMPI, bool isReturnToWL, string? cancelReason);
        public int CreatePastAppointment(DateTime appDate, string appTime, string appWith1, string appLocation, int iLinkedRef, int mpi,
            string appType, int duration, string sStaffCode, string outcome, bool isClockStop, string letterReq, int patientsSeen, string arrivalTime);
    }
    public class AppointmentSqlServices : IAppointmentSqlServices
    {
        private readonly IConfiguration _config;
        public AppointmentSqlServices(IConfiguration config) 
        {
            _config = config;
        }        
        public int CreateAppointment(DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation, 
            int iLinkedRef, int mpi, string appType, int duration, string sStaffCode, string sInstructions, int wlid, int slotID)
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
            cmd.Parameters.Add("@slotID", SqlDbType.Int).Value = slotID;
            cmd.Parameters.Add("@WLID", SqlDbType.Int).Value = wlid;
            var returnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int); //return success or not
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            var iReturnValue = (int)returnValue.Value;
            con.Close();

            return iReturnValue;
        }
        public void ModifyAppointment(int refID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sStaffCode, string sInstructions, string sCancellation, int famMPI, bool isReturnToWL, string? cancelReason)
        {
            DateTime dTim = DateTime.Parse("1899-12-30 " + appTime);

            if (appWith2 == null) { appWith2 = ""; }
            if (appWith3 == null) { appWith3 = ""; }
            if (sInstructions == null) { sInstructions = ""; }
            if (sCancellation == null) { sCancellation = ""; }
            if (cancelReason == null) { cancelReason = ""; }
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
            cmd.Parameters.Add("@cancellationReason", SqlDbType.VarChar).Value = cancelReason;
            cmd.Parameters.Add("@famMPI", SqlDbType.Int).Value = famMPI;
            cmd.Parameters.Add("@returnToWL", SqlDbType.Int).Value = returnToWL;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public int CreatePastAppointment(DateTime appDate, string appTime, string appWith1, string appLocation, int iLinkedRef, int mpi,
        string appType, int duration, string sStaffCode, string outcome, bool isClockStop, string letterReq, int patientsSeen, string arrivalTime)
        {
            DateTime dAppTim = DateTime.Parse("1899-12-30 " + appTime);
            DateTime dArrTim = DateTime.Parse("1899-12-30 " + arrivalTime);

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerCreatePastAppointment]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ApptDate", SqlDbType.DateTime).Value = appDate;
            cmd.Parameters.Add("@ApptTime", SqlDbType.DateTime).Value = dAppTim;
            cmd.Parameters.Add("@ApptWith1", SqlDbType.VarChar).Value = appWith1;
            cmd.Parameters.Add("@ApptLocation", SqlDbType.VarChar).Value = appLocation;
            cmd.Parameters.Add("@LinkedRefID", SqlDbType.BigInt).Value = iLinkedRef;
            cmd.Parameters.Add("@MPI", SqlDbType.BigInt).Value = mpi;
            cmd.Parameters.Add("@ApptType", SqlDbType.VarChar).Value = appType;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@UserStaffCode", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@counseled", SqlDbType.VarChar).Value = outcome;
            cmd.Parameters.Add("@clockStop", SqlDbType.Bit).Value = isClockStop;
            cmd.Parameters.Add("@letterRequired", SqlDbType.VarChar).Value = letterReq;
            cmd.Parameters.Add("@patientsSeen", SqlDbType.Int).Value = patientsSeen;
            cmd.Parameters.Add("@arrivalTime", SqlDbType.DateTime).Value = dArrTim;

            var returnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int); //return success or not
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.ExecuteNonQuery();
            var iReturnValue = (int)returnValue.Value;
            con.Close();

            return iReturnValue;
        }
    }    
}
