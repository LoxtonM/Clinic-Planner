using CPTest.Data;
using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IClinicLetterSqlServices
    {
        public void UpdateClinicLetter(int refID, string staffCode);
    }
    public class ClinicLetterSqlServices : IClinicLetterSqlServices
    {        
        private readonly IConfiguration _config;
        
        public ClinicLetterSqlServices(IConfiguration config)
        {            
            _config = config;
        }
                
        public void UpdateClinicLetter(int refID, string userName)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerSetClinicLetterPrinted]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RefID", SqlDbType.Int).Value = refID;
            cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = userName;
            cmd.ExecuteNonQuery();
            con.Close();            
        }
    }
}
