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
        private readonly DataConnections _dc;
        private readonly SqlServices _ss;

        public WLModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _dc = new DataConnections(_context);
            _ss = new SqlServices(_config);
        }

        public Patient patient { get; set; }
        public StaffMember staffMember { get; set; }
        public List<StaffMember> staffMemberList { get; set; }
        public ClinicVenue clinicVenue { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }        

        
        public void OnGet(int mpi, string clinicID, string clinicianID)
        {
            try
            {
                if (clinicianID != null)
                {
                    staffMember = _dc.GetStaffDetails(clinicianID);
                }

                if (clinicID != null)
                {
                    clinicVenue = _dc.GetVenueDetails(clinicID);
                }

                if (mpi != null)
                {
                    patient = _dc.GetPatientDetails(mpi);
                }

                staffMemberList = _dc.GetStaffMemberList();

                clinicVenueList = _dc.GetVenueList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
        
        public void OnPost(int mpi, string clinicianID, string clinicID, string sOldClinicianID, string sOldClinicID, bool isRemoval)
        {
            try
            {
                if (clinicianID != null)
                {
                    staffMember = _dc.GetStaffDetails(clinicianID);
                }

                if (clinicID != null)
                {
                    clinicVenue = _dc.GetVenueDetails(clinicID);
                }

                if (mpi != null)
                {
                    patient = _dc.GetPatientDetails(mpi);
                }
                string sUsername = "LoxM";

                staffMemberList = _dc.GetStaffMemberList();

                clinicVenueList = _dc.GetVenueList();

                _ss.ModifyWaitingListEntry(mpi, clinicianID, clinicID, sOldClinicianID, sOldClinicID, sUsername, isRemoval);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
