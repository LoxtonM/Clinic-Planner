using CPTest.Data;
using CPTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IClinicPatternSqlServices
    {       
        public void SaveClinicPattern(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate, string username);
        public void UpdateClinicPattern(int iPatternID, string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate, string username);
     
    }
    public class ClinicPatternSqlServices : IClinicPatternSqlServices
    {
        private readonly IConfiguration _config;
        private readonly IClinicSlotsCreator _csc;
        private readonly IPatternData _patternData;        
        private readonly DataContext _context;
        public ClinicPatternSqlServices(DataContext context, IConfiguration config) 
        {
            _context = context;
            _config = config;
            _csc = new ClinicSlotsCreator(_context, _config);
            _patternData = new PatternData(_context);
        }        
        
        public void SaveClinicPattern(string clinicianID, string clinicID, int dayofWeek, int weekofMonth, 
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin, 
                DateTime dStartDate, DateTime? dEndDate, string username)
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
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = startHour;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = startMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@startdate", SqlDbType.DateTime).Value = dStartDate;
            cmd.Parameters.Add("@enddate", SqlDbType.DateTime).Value = dEndDate;
            cmd.ExecuteNonQuery();
            con.Close();

            int patternID = _patternData.GetPatternDetailsByData(clinicianID, clinicID, dayofWeek, weekofMonth, sMonthofYear, numSlots, duration,
                startHour, startMin, dStartDate).PatternID;

            _csc.SetupClinicPattern(patternID, clinicianID, clinicID, dayofWeek, weekofMonth, sMonthofYear, numSlots, duration, startHour, 
                startMin, dStartDate, dEndDate, username); 
        }
        
        public void UpdateClinicPattern(int patternID, string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate, string username)
        {
            if(dEndDate == null)
            {
                dEndDate = DateTime.Parse("1900-01-01");
            }

            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerUpdatePattern]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@patternID", SqlDbType.Int).Value = patternID;            
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

            _csc.SetupClinicPattern(patternID, clinicianID, clinicID, dayofWeek, weekofMonth, sMonthofYear, numSlots, duration, iStartHour,
                startMin, dStartDate, dEndDate, username); //we can't simply pass the pattern ID to it, because it refuses to get the new value!
        }        
        
    }
}
