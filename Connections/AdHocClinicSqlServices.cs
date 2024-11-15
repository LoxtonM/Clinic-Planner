using CPTest.Data;
using System.Data;
using System.Data.SqlClient;
using ClinicalXPDataConnections.Data;

namespace CPTest.Connections
{
    interface IAdHocClinicSqlServices
    {               
       
        public void SaveAdHocClinic(string clinicianID, string clinicID, int numSlots, int duration, int iStartHour, int startMin,
                DateTime dClinicDate, string username);       
        public void UpdateAdHocClinic(int ID, string clinicianID, string clinicID, int duration, int startHr, int startMin, int numSlots, DateTime dClinicDate, string username);
       
    }
    public class AdHocClinicSqlServices : IAdHocClinicSqlServices
    {
        private readonly IConfiguration _config;
        private readonly IClinicSlotsCreator _csc;
        private readonly IAdHocClinicData _adhocClinicData;
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        public AdHocClinicSqlServices(ClinicalContext context, CPXContext cPXContext, IConfiguration config) 
        {
            _context = context;
            _cpxContext = cPXContext;
            _config = config;
            _csc = new ClinicSlotsCreator(_context, _cpxContext, _config);
            _adhocClinicData = new AdHocClinicData(_cpxContext);
        }        
                
        public void SaveAdHocClinic(string clinicianID, string clinicID, int numSlots, int duration, int startHr, int startMin,
                DateTime dClinicDate, string username)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSaveAdHocClinic]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@clinician", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@clinic", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@numslots", SqlDbType.Int).Value = numSlots;
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = startHr;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = startMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@clinicdate", SqlDbType.DateTime).Value = dClinicDate;            
            cmd.ExecuteNonQuery();
            con.Close();

            int adHocID = _adhocClinicData.GetAdHocClinicDetailsByData(clinicianID, clinicID, numSlots, duration, startHr, startMin, dClinicDate).ID;

            _csc.SetupAdHocClinic(adHocID, dClinicDate, startHr, startMin, clinicianID, clinicID, duration, numSlots, username);
        }
       
        public void UpdateAdHocClinic(int id, string clinicianID, string clinicID, int duration, int startHr, int startMin, int numSlots, DateTime dClinicDate, string username)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerUpdateAdHocClinic]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@adhocID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@numslots", SqlDbType.Int).Value = numSlots;
            cmd.Parameters.Add("@starthour", SqlDbType.Int).Value = startHr;
            cmd.Parameters.Add("@startmin", SqlDbType.Int).Value = startMin;
            cmd.Parameters.Add("@duration", SqlDbType.Int).Value = duration;
            cmd.Parameters.Add("@clinicdate", SqlDbType.DateTime).Value = dClinicDate;
            cmd.ExecuteNonQuery();
            con.Close();

            _csc.SetupAdHocClinic(id, dClinicDate, startHr, startMin, clinicianID, clinicID, duration, numSlots, username);
        }
       
    }
}
