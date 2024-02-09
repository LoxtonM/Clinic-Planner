using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class WLModifyModel : PageModel
    {

        private readonly DataContext _context;
        private readonly IConfiguration _config;
        DataConnections dc;
        SqlServices ss;

        public WLModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            dc = new DataConnections(_context);
            ss = new SqlServices(_config);
        }

        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }        

        
        public void OnGet(int iMPI, string sClinicID, string sClinicianID)
        {
            try
            {
                if (sClinicianID != null)
                {
                    staffMember = dc.GetStaffDetails(sClinicianID);
                }

                if (sClinicID != null)
                {
                    clinicVenue = dc.GetVenueDetails(sClinicID);
                }

                if (iMPI != null)
                {
                    patient = dc.GetPatientDetails(iMPI);
                }

                staffMemberList = dc.GetStaffMemberList();

                clinicVenueList = dc.GetVenueList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
        
        public void OnPost(int iMPI, string sClinicianID, string sClinicID, string sOldClinicianID, string sOldClinicID, bool isRemoval)
        {
            try
            {
                if (sClinicianID != null)
                {
                    staffMember = dc.GetStaffDetails(sClinicianID);
                }

                if (sClinicID != null)
                {
                    clinicVenue = dc.GetVenueDetails(sClinicID);
                }

                if (iMPI != null)
                {
                    patient = dc.GetPatientDetails(iMPI);
                }
                string sUsername = "LoxM";

                staffMemberList = dc.GetStaffMemberList();

                clinicVenueList = dc.GetVenueList();

                ss.ModifyWaitingListEntry(iMPI, sClinicianID, sClinicID, sOldClinicianID, sOldClinicID, sUsername, isRemoval);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
