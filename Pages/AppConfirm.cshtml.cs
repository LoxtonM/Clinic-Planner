using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AppConfirmModel : PageModel
    {
        private readonly DataContext _context;

        public AppConfirmModel(DataContext context)
        {
            _context = context;
        }
        public Patient? Patient { get; set; }
        public StaffMember? StaffMember { get; set; }

        public ClinicVenue? ClinicVenue { get; set; }

        public DateTime AppDate;
        public DateTime AppTime;
        public int AppDur;
        public string? AppDateString;
        public string? AppTimeString;
        public void OnGet(string sMPI, string sClin, string sVen, string sDat, string sTim, string sDur)
        {
            int iMPI = Int32.Parse(sMPI);

            Patient = _context.Patients.FirstOrDefault(p => p.MPI == iMPI);

            StaffMember = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == sClin);

            ClinicVenue = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == sVen);

            AppDateString = sDat;
            AppTimeString = sTim;

            AppDate = DateTime.Parse(sDat);
            AppTime = DateTime.Parse("1899-12-30 " + sTim);
            AppDur = Int32.Parse(sDur);
        }

        public void OnPost(int iMPI, string sClin, string sVen, DateTime dDat, string sTim, int iDur)
        {
            //placeholder
           Response.Redirect("Finished"); 
        }
    }
}
