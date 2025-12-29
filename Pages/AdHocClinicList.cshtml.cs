using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using System.Threading.Tasks;

namespace CPTest.Pages
{
    public class AdHocClinicListModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly IStaffUserDataAsync _staffData;
        private readonly IAdHocClinicDataAsync _adHocClinicData;
        public AdHocClinicListModel(ClinicalContext context, CPXContext cpxContext)
        {
            _context = context;
            _cpxContext = cpxContext;
            _staffData = new StaffUserDataAsync(_context);
            _adHocClinicData = new AdHocClinicDataAsync(_cpxContext);
        }
        
        public StaffMember clinician;
        public List<ClinicsAdded> adHocList { get; set; }
        public async Task OnGet(string staffCode)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }

                clinician = await _staffData.GetStaffMemberDetailsByStaffCode(staffCode);
                adHocList = await _adHocClinicData.GetAdHocList(staffCode);
                adHocList = adHocList.Where(c => c.ClinicDate >= DateTime.Now.Date).OrderByDescending(c => c.ClinicDate).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }    
    }
}
