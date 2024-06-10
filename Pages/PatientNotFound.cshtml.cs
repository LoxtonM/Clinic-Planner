using CPTest.Connections;
using CPTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class PatientNotFoundModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IWaitingListSqlServices _ss;

        public PatientNotFoundModel(IConfiguration config)
        {
            _config = config;
            _ss = new WaitingListSqlServices(_config);
        }

        public int wlIntID { get; set; }
        public string wlClinicianID { get; set; }
        public string wlClinicID { get; set; }

        public void OnGet(int intID, string clinicianID, string clinicID)
        {
            if (User.Identity.Name is null)
            {
                Response.Redirect("Login");
            }

            wlIntID = intID;
            wlClinicianID = clinicianID;
            wlClinicID = clinicID;
        }

        public void OnPost(int intID, string clinicianID, string clinicID)
        {
            _ss.ModifyWaitingListEntry(intID, clinicianID, clinicID, "", "", "", true);

            Response.Redirect("Index");
        }
    }
}
