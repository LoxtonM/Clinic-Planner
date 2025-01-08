using CPTest.Connections;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace CPTest.Connections
{
    interface IAuditSqlServices
    {
        public void CreateAudit(string staffCode, string formName, string searchTerm, string ipAddress);
        public void WriteAuditUpdate(string staffCode, string formName, string recordKey, string tableName, string oldValue, string newValue, string ipAddress);
    }
    public class AuditSqlServices : IAuditSqlServices
    {
        private readonly IConfiguration _config;
        public AuditSqlServices(IConfiguration config) 
        {
            _config = config;
        }        
        public void CreateAudit(string staffCode, string formName, string searchTerm, string ipAddress)
        {


            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));            
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_CreateAudit]", con);
            cmd.CommandType = CommandType.StoredProcedure;            
            cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@form", SqlDbType.VarChar).Value = formName;
            cmd.Parameters.Add("@database", SqlDbType.VarChar).Value = "CP-X";
            //cmd.Parameters.Add("@machine", SqlDbType.VarChar).Value = System.Environment.MachineName;
            cmd.Parameters.Add("@machine", SqlDbType.VarChar).Value = Dns.GetHostByAddress(ipAddress).HostName.Substring(0,10);
            cmd.Parameters.Add("@searchTerm", SqlDbType.VarChar).Value = searchTerm;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void WriteAuditUpdate(string staffCode, string formName, string recordKey, string tableName, string oldValue, string newValue, string ipAddress)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_WriteAuditUpdate]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@form", SqlDbType.VarChar).Value = formName;
            cmd.Parameters.Add("@recordkey", SqlDbType.VarChar).Value = recordKey;
            cmd.Parameters.Add("@tableName", SqlDbType.VarChar).Value = tableName;
            cmd.Parameters.Add("@machine", SqlDbType.VarChar).Value = Dns.GetHostByAddress(ipAddress).HostName.Substring(0, 10);
            cmd.Parameters.Add("@oldValue", SqlDbType.VarChar).Value = oldValue;
            cmd.Parameters.Add("@newValue", SqlDbType.VarChar).Value = newValue;
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
