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
        DataConnections dc;

        public WLModifyModel(DataContext context)
        {
            _context = context;
            dc = new DataConnections(_context);
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
        
        public void OnPost(int iMPI, string sClinicID, string sClinicianID)
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
    }
}
