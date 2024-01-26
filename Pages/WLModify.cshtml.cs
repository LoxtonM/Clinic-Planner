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

        public Patient Patient { get; set; }
        public StaffMember StaffMember { get; set; }
        public List<StaffMember> StaffMembers { get; set; }
        public ClinicVenue ClinicVenue { get; set; }
        public List<ClinicVenue> ClinicVenues { get; set; }        

        
        public void OnGet(int iMPI, string sClinicID, string sClinicianID)
        {
            try
            {
                if (sClinicianID != null)
                {
                    StaffMember = dc.GetStaffDetails(sClinicianID);
                }

                if (sClinicID != null)
                {
                    ClinicVenue = dc.GetVenueDetails(sClinicID);
                }

                if (iMPI != null)
                {
                    Patient = dc.GetPatientDetails(iMPI);
                }

                StaffMembers = dc.GetStaffMemberList();

                ClinicVenues = dc.GetVenueList();
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
                    StaffMember = dc.GetStaffDetails(sClinicianID);
                }

                if (sClinicID != null)
                {
                    ClinicVenue = dc.GetVenueDetails(sClinicID);
                }

                if (iMPI != null)
                {
                    Patient = dc.GetPatientDetails(iMPI);
                }

                StaffMembers = dc.GetStaffMemberList();

                ClinicVenues = dc.GetVenueList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
