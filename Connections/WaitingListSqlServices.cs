using System.Data;
using System.Data.SqlClient;

namespace CPTest.Connections
{
    interface IWaitingListSqlServices
    {        
        public void CreateWaitingListEntry(int mpi, string clinicianID, string clinicID, string staffCode, int priorityLevel, int linkedRef);
        public void ModifyWaitingListEntry(int intID, string clinicianID, string clinicID, int priorityLevel, string oldClinicianID, string oldClinicID,
             int oldPriorityLevel, string staffCode, bool isRemoval);
    }
    public class WaitingListSqlServices : IWaitingListSqlServices
    {
        private readonly IConfiguration _config;
        public WaitingListSqlServices(IConfiguration config) 
        {
            _config = config;
        }        
        
        public void CreateWaitingListEntry(int mpi, string clinicianID, string clinicID, string staffCode, int priorityLevel, int linkedRef)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerCreateWaitingListEntry]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@MPI", SqlDbType.Int).Value = mpi;
            cmd.Parameters.Add("@ClinicianID", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@ClinicID", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@refID", SqlDbType.Int).Value = linkedRef;
            cmd.Parameters.Add("@priorityLevel", SqlDbType.Int).Value = priorityLevel;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = staffCode;
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public void ModifyWaitingListEntry(int intID, string clinicianID, string clinicID, int priorityLevel, string oldClinicianID, string oldClinicID,
            int oldPriorityLevel, string staffCode, bool isRemoval)
        {
            //since there is no primary key on the Waiting List table, we need the old values to be able to update!
            if(oldClinicianID == null) { oldClinicianID = ""; }
            if (oldClinicID == null) { oldClinicID = ""; }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("ConString"));
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.[sp_ClinicPlannerModifyWaitingListEntry]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@IntID", SqlDbType.Int).Value = intID;
            cmd.Parameters.Add("@ClinicianID", SqlDbType.VarChar).Value = clinicianID;
            cmd.Parameters.Add("@ClinicID", SqlDbType.VarChar).Value = clinicID;
            cmd.Parameters.Add("@PriorityLevel", SqlDbType.Int).Value = priorityLevel;
            cmd.Parameters.Add("@OldClinicianID", SqlDbType.VarChar).Value = oldClinicianID;
            cmd.Parameters.Add("@OldClinicID", SqlDbType.VarChar).Value = oldClinicID;
            cmd.Parameters.Add("@OldPriorityLevel", SqlDbType.Int).Value = oldPriorityLevel;
            cmd.Parameters.Add("@userStaffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@MachineName", SqlDbType.VarChar).Value = System.Environment.MachineName;
            cmd.Parameters.Add("@isRemoval", SqlDbType.Bit).Value = isRemoval;
            cmd.ExecuteNonQuery();
            con.Close();
        }            
    }
}
