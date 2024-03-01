using CPTest.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace CPTest.Connections
{
    public class SqlServices
    {
        private readonly IConfiguration _config;
        public SqlServices(IConfiguration config) 
        {
            _config = config;
        }
        
        public void CreateAppointment(DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation, 
            int iLinkedRef, int iMPI, string appType, int iDuration, string sStaffCode, string sInstructions)
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
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ModifyAppointment(int iRefID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int iDuration, string sStaffCode, string sInstructions, string sCancellation)
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
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.Parameters.Add("@cancellation", SqlDbType.VarChar).Value = sCancellation;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ModifyWaitingListEntry(int iMPI, string sClinicianID, string sClinicID, string sOldClinicianID, string sOldClinicID, 
            string sStaffCode, bool isRemoval)
        {
            //since there is no primary key on the Waiting List table, we need the old values to be able to update!
            if(sOldClinicianID == null) { sOldClinicianID = ""; }
            if (sOldClinicID == null) { sOldClinicID = ""; }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerModifyWaitingListEntry]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MPI", SqlDbType.Int).Value = iMPI;
            cmd.Parameters.Add("@ClinicianID", SqlDbType.VarChar).Value = sClinicianID;
            cmd.Parameters.Add("@ClinicID", SqlDbType.VarChar).Value = sClinicID;
            cmd.Parameters.Add("@OldClinicianID", SqlDbType.VarChar).Value = sOldClinicianID;
            cmd.Parameters.Add("@OldClinicID", SqlDbType.VarChar).Value = sOldClinicID;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@isRemoval", SqlDbType.Bit).Value = isRemoval;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void CreateClinicSlot(DateTime dSlotDate, DateTime dSlotTime, string sClinicianID, 
            string sClinicID, string sStaffCode, int iDuration, int iPatternID)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerCreateClinicSlot]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = sClinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = sClinicID;
            cmd.Parameters.Add("@slotDate", SqlDbType.DateTime).Value = dSlotDate;
            cmd.Parameters.Add("@slotTime", SqlDbType.DateTime).Value = dSlotTime;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = iDuration;
            cmd.Parameters.Add("@patternID", SqlDbType.Int).Value = iPatternID;
            cmd.Parameters.Add("@staffcode", SqlDbType.VarChar).Value = sStaffCode;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void SaveClinicPattern(string sClinicianID, string sClinicID, int iDayofWeek, int iWeekofMonth, 
                string sMonthofYear, int iNumSlots, int iDuration, int iStartHour, int iStartMin, 
                DateTime dStartDate, DateTime? dEndDate)
        {
            if(dEndDate == null || dEndDate == DateTime.Parse("0001-01-01"))
            {
                dEndDate = DateTime.Parse("1900-01-01");
            }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSavePattern]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = sClinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = sClinicID;
            cmd.Parameters.Add("@dayofweek", SqlDbType.Int).Value = iDayofWeek;
            cmd.Parameters.Add("@weekofmonth", SqlDbType.Int).Value = iWeekofMonth;
            cmd.Parameters.Add("@monthofyear", SqlDbType.VarChar).Value = sMonthofYear;
            cmd.Parameters.Add("@numslots", SqlDbType.Int).Value = iNumSlots;
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = iStartHour;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = iStartMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = iDuration;
            cmd.Parameters.Add("@startdate", SqlDbType.DateTime).Value = dStartDate;
            cmd.Parameters.Add("@enddate", SqlDbType.DateTime).Value = dEndDate;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void SaveAdHocClinic(string sClinicianID, string sClinicID, int iNumSlots, int iDuration, int iStartHour, int iStartMin,
                DateTime dClinicDate)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSaveAdHocClinic]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = sClinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = sClinicID;
            cmd.Parameters.Add("@numslots", SqlDbType.Int).Value = iNumSlots;
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = iStartHour;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = iStartMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = iDuration;
            cmd.Parameters.Add("@clinicdate", SqlDbType.DateTime).Value = dClinicDate;            
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ClearClinicSlots(string sClinicianID, string sClinicID, DateTime? dEndDate)
        {
            if (dEndDate == null || dEndDate == DateTime.Parse("0001-01-01"))
            {
                dEndDate = DateTime.Parse("1900-01-01");
            }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerClearSlots]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = sClinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = sClinicID;
            cmd.Parameters.Add("@enddate", SqlDbType.DateTime).Value = dEndDate;
            //cmd.Parameters.Add("@staffcode", SqlDbType.VarChar).Value = sStaffCode;
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }
}
