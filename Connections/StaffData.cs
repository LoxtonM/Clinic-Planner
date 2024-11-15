using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;

namespace CPTest.Connections
{
    interface IStaffData
    {
        public StaffMember GetStaffDetails(string clin);
        public StaffMember GetStaffDetailsByUsername(string username);
        public List<StaffMember> GetStaffMemberList();
    }
    public class StaffData : IStaffData
    {
        private readonly ClinicalContext _context;
        public StaffData(ClinicalContext context)
        {
            _context = context;
        }
              

        public StaffMember GetStaffDetails(string clin)
        {
            StaffMember staff = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == clin);
            return staff;
        }

        public StaffMember GetStaffDetailsByUsername(string username)
        {
            StaffMember staff = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == username);
            return staff;
        }

        public List<StaffMember> GetStaffMemberList() 
        {            
            IQueryable<StaffMember> stafflist = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).OrderBy(s => s.NAME);
            return stafflist.ToList();
        }

        
    }
}
