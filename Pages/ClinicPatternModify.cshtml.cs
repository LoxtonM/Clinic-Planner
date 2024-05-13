using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicPatternModifyModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IPatternData _patternData;
        private readonly SqlServices _sql;

        public ClinicPatternModifyModel(DataContext context, IConfiguration config)
        {
            _context = context;            
            _config = config;
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _patternData = new PatternData(_context);
            _sql = new SqlServices(_config);
        }

        public ClinicPattern pattern { get; set; }
        public StaffMember clinician { get; set; }
        public ClinicVenue venue { get; set; }

        public void OnGet(int id)
        {
            try
            {
                pattern = _patternData.GetPatternDetails(id);
                clinician = _staffData.GetStaffDetails(pattern.StaffID);
                venue = _clinicVenueData.GetVenueDetails(pattern.Clinic);
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
                pattern = _patternData.GetPatternDetails(id);
                clinician = _staffData.GetStaffDetails(pattern.StaffID);
                venue = _clinicVenueData.GetVenueDetails(pattern.Clinic);

                _sql.UpdateClinicPattern(id); //, day, week, months, dur, startHr, startMin, numSlots, dStart, dEnd);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
