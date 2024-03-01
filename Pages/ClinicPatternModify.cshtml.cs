using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicPatternModifyModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        DataConnections dc;
        SqlServices sql;

        public ClinicPatternModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;            
            _config = config;
            dc = new DataConnections(_context);
            sql = new SqlServices(_config);
        }

        public ClinicPattern pattern { get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }

        public void OnGet(int ID)
        {
            try
            {
                pattern = dc.GetPatternDetails(ID);
                clinician = dc.GetStaffDetails(pattern.StaffID);
                venue = dc.GetVenueDetails(pattern.Clinic);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }        
        }

        public void OnPost(int ID, int iDay, int iWeek, string sMonths, int iDur,
            int iStartHr, int iStartMin, int iNumSlots, DateTime dStart, DateTime? dEnd)
        {
            try
            {
                pattern = dc.GetPatternDetails(ID);
                clinician = dc.GetStaffDetails(pattern.StaffID);
                venue = dc.GetVenueDetails(pattern.Clinic);

                sql.UpdateClinicPattern(ID); //, iDay, iWeek, sMonths, iDur, iStartHr, iStartMin, iNumSlots, dStart, dEnd);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
