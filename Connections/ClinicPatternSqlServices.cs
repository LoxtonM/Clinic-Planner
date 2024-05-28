using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IClinicPatternSqlServices
    {       
        public void SaveClinicPattern(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate);
        public void UpdateClinicPattern(int iPatternID);
     
    }
    public class ClinicPatternSqlServices : IClinicPatternSqlServices
    {
        private readonly IConfiguration _config;
        public ClinicPatternSqlServices(IConfiguration config) 
        {
            _config = config;
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
        
    }
}
