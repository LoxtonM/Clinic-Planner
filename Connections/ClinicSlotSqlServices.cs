using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IClinicSlotSqlServices
    {
       
        public void CreateClinicSlot(DateTime dSlotDate, DateTime dSlotTime, string clinicianID,
            string clinicID, string sStaffCode, int duration, int iPatternID);        
        public void ClearClinicSlots(string clinicianID, string clinicID, DateTime? dEndDate);
        public void ModifyClinicSlot(int slotID, string staffCode, string action, string? details = "");
        public void DeleteClinicSlot(int slotID, string staffCode);
    }
    public class ClinicSlotSqlServices : IClinicSlotSqlServices
    {
        private readonly IConfiguration _config;
        public ClinicSlotSqlServices(IConfiguration config) 
        {
            _config = config;
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

        public void ModifyClinicSlot(int slotID, string staffCode, string action, string? details="")
        {            
            if (action == "Unavail" || action == "Open")
            {
                SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
                con.Open();
                SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSetAvailability]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@slotID", SqlDbType.Int).Value = slotID;
                cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = action;
                cmd.ExecuteNonQuery();
                con.Close();
                
            }
            else if (action == "Reserve")
            {
                SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
                con.Open();
                SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSetReserved]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@slotID", SqlDbType.Int).Value = slotID;
                cmd.Parameters.Add("@cguNumber", SqlDbType.VarChar).Value = details;
                cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            else if (action == "ForMeOnly")
            {
                //todo
               
            }
        }

        public void DeleteClinicSlot(int slotID, string staffCode) 
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerDeleteSlot]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@slotID", SqlDbType.Int).Value = slotID;
            cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.ExecuteNonQuery();
            con.Close();
            //[sp_ClinicPlannerDeleteSlot]
        }
    }
}
