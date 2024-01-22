using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AppModifyModel : PageModel
    {

        private readonly DataContext _context;

        public AppModifyModel(DataContext context)
        {
            _context = context;
        }
        public Patient Patient { get; set; }
        public StaffMember StaffMember { get; set; }
        public List<StaffMember> StaffMembers { get; set; }
        public ClinicVenue ClinicVenue { get; set; }
        public List<ClinicVenue> ClinicVenues { get; set; }
        public Appointment Appointment { get; set; }

        
        public void OnGet(string sRefID)
        {
            try
            {
                int iRefID = Int32.Parse(sRefID);
                Appointment = _context.Appointments.FirstOrDefault(a => a.RefID == iRefID);

                StaffMember = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == Appointment.STAFF_CODE_1);

                ClinicVenue = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == Appointment.FACILITY);

                Patient = _context.Patients.FirstOrDefault(p => p.MPI == Appointment.MPI);

                StaffMembers = _context.StaffMembers.Where(s => s.InPost == true & s.Clinical == true).ToList();

                ClinicVenues = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }        
    }
}
