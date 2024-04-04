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
        private readonly DataConnections _dc;
        private readonly SqlServices _sql;

        public ClinicPatternModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;            
            _config = config;
            _dc = new DataConnections(_context);
            _sql = new SqlServices(_config);
        }

        public ClinicPattern pattern { get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }

        public void OnGet(int id)
        {
            try
            {
                pattern = _dc.GetPatternDetails(id);
                clinician = _dc.GetStaffDetails(pattern.StaffID);
                venue = _dc.GetVenueDetails(pattern.Clinic);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }        
        }

        public void OnPost(int id, int day, int week, string months, int dur,
            int startHr, int startMin, int numSlots, DateTime dStart, DateTime? dEnd)
        {
            try
            {
                pattern = _dc.GetPatternDetails(id);
                clinician = _dc.GetStaffDetails(pattern.StaffID);
                venue = _dc.GetVenueDetails(pattern.Clinic);

                _sql.UpdateClinicPattern(id); //, day, week, months, dur, startHr, startMin, numSlots, dStart, dEnd);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
