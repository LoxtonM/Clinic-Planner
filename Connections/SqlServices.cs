using CPTest.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ModifyAppointment(int refID, DateTime appDate, string appTime, string appWith1, string appWith2, string appWith3, string appLocation,
            string appType, int duration, string sStaffCode, string sInstructions, string sCancellation)
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
            cmd.Parameters.Add("@RefID", SqlDbType.Int).Value = refID;
            cmd.Parameters.Add("@ApptDate", SqlDbType.DateTime).Value = appDate;
            cmd.Parameters.Add("@ApptTime", SqlDbType.DateTime).Value = dTim;
            cmd.Parameters.Add("@ApptWith1", SqlDbType.VarChar).Value = appWith1;
            cmd.Parameters.Add("@ApptWith2", SqlDbType.VarChar).Value = appWith2;
            cmd.Parameters.Add("@ApptWith3", SqlDbType.VarChar).Value = appWith3;
            cmd.Parameters.Add("@ApptLocation", SqlDbType.VarChar).Value = appLocation;            
            cmd.Parameters.Add("@ApptType", SqlDbType.VarChar).Value = appType;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = sStaffCode;
            cmd.Parameters.Add("@instructions", SqlDbType.VarChar).Value = sInstructions;
            cmd.Parameters.Add("@cancellation", SqlDbType.VarChar).Value = sCancellation;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void CreateWaitingListEntry(int mpi, string clinicianID, string clinicID, string staffCode)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerCreateWaitingListEntry]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MPI", SqlDbType.Int).Value = mpi;
            cmd.Parameters.Add("@ClinicianID", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@ClinicID", SqlDbType.VarChar).Value = clinicID;            
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = staffCode;            
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ModifyWaitingListEntry(int mpi, string clinicianID, string clinicID, string sOldClinicianID, string sOldClinicID, 
            string staffCode, bool isRemoval)
        {
            //since there is no primary key on the Waiting List table, we need the old values to be able to update!
            if(sOldClinicianID == null) { sOldClinicianID = ""; }
            if (sOldClinicID == null) { sOldClinicID = ""; }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerModifyWaitingListEntry]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MPI", SqlDbType.Int).Value = mpi;
            cmd.Parameters.Add("@ClinicianID", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@ClinicID", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@OldClinicianID", SqlDbType.VarChar).Value = sOldClinicianID;
            cmd.Parameters.Add("@OldClinicID", SqlDbType.VarChar).Value = sOldClinicID;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@isRemoval", SqlDbType.Bit).Value = isRemoval;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void CreateClinicSlot(DateTime dSlotDate, DateTime dSlotTime, string clinicianID, 
            string clinicID, string sStaffCode, int duration, int iPatternID)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerCreateClinicSlot]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@slotDate", SqlDbType.DateTime).Value = dSlotDate;
            cmd.Parameters.Add("@slotTime", SqlDbType.DateTime).Value = dSlotTime;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@patternID", SqlDbType.Int).Value = iPatternID;
            cmd.Parameters.Add("@staffcode", SqlDbType.VarChar).Value = sStaffCode;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void SaveClinicPattern(string clinicianID, string clinicID, int dayofWeek, int weekofMonth, 
                string sMonthofYear, int numSlots, int duration, int iStartHour, int startMin, 
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
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@dayofweek", SqlDbType.Int).Value = dayofWeek;
            cmd.Parameters.Add("@weekofmonth", SqlDbType.Int).Value = weekofMonth;
            cmd.Parameters.Add("@monthofyear", SqlDbType.VarChar).Value = sMonthofYear;
            cmd.Parameters.Add("@numslots", SqlDbType.Int).Value = numSlots;
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = iStartHour;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = startMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@startdate", SqlDbType.DateTime).Value = dStartDate;
            cmd.Parameters.Add("@enddate", SqlDbType.DateTime).Value = dEndDate;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void SaveAdHocClinic(string clinicianID, string clinicID, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dClinicDate)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSaveAdHocClinic]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@numslots", SqlDbType.Int).Value = numSlots;
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = iStartHour;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = startMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@clinicdate", SqlDbType.DateTime).Value = dClinicDate;            
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void UpdateClinicPattern(int iPatternID)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerUpdateClinicPattern]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PatternID", SqlDbType.Int).Value = iPatternID;
            
            //cmd.ExecuteNonQuery();
            con.Close();
        }
        public void UpdateAdHocClinic(int ID)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerUpdateAdHocClinic]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;

            //cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ClearClinicSlots(string clinicianID, string clinicID, DateTime? dEndDate)
        {
            if (dEndDate == null || dEndDate == DateTime.Parse("0001-01-01"))
            {
                dEndDate = DateTime.Parse("1900-01-01");
            }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerClearSlots]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@enddate", SqlDbType.DateTime).Value = dEndDate;
            //cmd.Parameters.Add("@staffcode", SqlDbType.VarChar).Value = sStaffCode;
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }
}
