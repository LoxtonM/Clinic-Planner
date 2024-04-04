using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CPTest.Pages
{
    public class AppConfirmModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config; 
        private readonly DataConnections _dc;
        private readonly SqlServices _ss;

        public AppConfirmModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _dc = new DataConnections(_context);
            _ss = new SqlServices(_config);            
        }

        public Patient? Patient { get; set; }
        public StaffMember? staffMember { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public List<Referral> linkedRefList { get; set; }
        public List<AppType> appTypeList { get; set; }

        public DateTime appDate;
        public DateTime appTime;
        public int appDur;
        public string? appDateString;
        public string? appTimeString;
        public string? appTypeDef;
        
        public void OnGet(string mpiString, string clin, string ven, string dat, string tim, string dur, string instructions)
        {
            try
            {
                int mpi = Int32.Parse(mpiString);

                Patient = _dc.GetPatientDetails(mpi);
                appTypeList = _dc.GetAppTypeList();
                staffMember = _dc.GetStaffDetails(clin);

                clinicVenue = _dc.GetVenueDetails(ven);

                linkedRefList = _dc.GetReferralsList(mpi);

                appDateString = dat;
                appTimeString = tim;

                appDate = DateTime.Parse(dat);
                appTime = DateTime.Parse("1899-12-30 " + tim);
                appDur = Int32.Parse(dur);

                if (staffMember.CLINIC_SCHEDULER_GROUPS == "GC")
                {
                    appTypeDef = "GC Only Appt";
                }
                else
                {
                    appTypeDef = "Cons. Appt";
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int mpi, int refID, string clin, string ven, DateTime dat, string tim, int dur, string instructions, string type)
        {
            try
            {
                string staffCode;

                Patient = _dc.GetPatientDetails(mpi);
                appTypeList = _dc.GetAppTypeList();
                staffMember = _dc.GetStaffDetails(clin);
                staffCode = _dc.GetStaffDetailsByUsername("mnln").STAFF_CODE; //placeholder - will replace when login screen available                
                
                clinicVenue = _dc.GetVenueDetails(ven);

                linkedRefList = _dc.GetReferralsList(mpi);

                _ss.CreateAppointment(dat, tim, clin, null, null, ven, refID, mpi, type, dur, staffCode, instructions);
                
                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
