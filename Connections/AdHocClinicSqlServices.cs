using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IAdHocClinicSqlServices
    {               
       
        public void SaveAdHocClinic(string clinicianID, string clinicID, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dClinicDate);       
        public void UpdateAdHocClinic(int ID);
       
    }
    public class AdHocClinicSqlServices : IAdHocClinicSqlServices
    {
        private readonly IConfiguration _config;
        public AdHocClinicSqlServices(IConfiguration config) 
        {
            _config = config;
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
       
    }
}
