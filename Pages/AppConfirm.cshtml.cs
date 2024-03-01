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
        DataConnections dc;
        SqlServices ss;

        public AppConfirmModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            dc = new DataConnections(_context);
            ss = new SqlServices(_config);            
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
        
        public void OnGet(string sMPI, string sClin, string sVen, string sDat, string sTim, string sDur, string sInstructions)
        {
            try
            {
                int iMPI = Int32.Parse(sMPI);

                Patient = dc.GetPatientDetails(iMPI);
                appTypeList = dc.GetAppTypeList();
                staffMember = dc.GetStaffDetails(sClin);

                clinicVenue = dc.GetVenueDetails(sVen);

                linkedRefList = dc.GetReferralsList(iMPI);

                appDateString = sDat;
                appTimeString = sTim;

                appDate = DateTime.Parse(sDat);
                appTime = DateTime.Parse("1899-12-30 " + sTim);
                appDur = Int32.Parse(sDur);

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

        public void OnPost(int iMPI, int iRefID, string sClin, string sVen, DateTime dDat, string sTim, int iDur, string sInstructions, string sType)
        {
            try
            {
                string sStaffCode;

                Patient = dc.GetPatientDetails(iMPI);
                appTypeList = dc.GetAppTypeList();
                staffMember = dc.GetStaffDetails(sClin);
                sStaffCode = dc.GetStaffDetailsByUsername("mnln").STAFF_CODE; //placeholder - will replace when login screen available                
                
                clinicVenue = dc.GetVenueDetails(sVen);

                linkedRefList = dc.GetReferralsList(iMPI);

                ss.CreateAppointment(dDat, sTim, sClin, null, null, sVen, iRefID, iMPI, sType, iDur, sStaffCode, sInstructions);
                
                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
