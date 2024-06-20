using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicSetupModel : PageModel
    {

        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IClinicPatternSqlServices _ssPat;
        private readonly IAdHocClinicSqlServices _ssAdHoc;
        private readonly IAuditSqlServices _audit;

        public ClinicSetupModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _ssPat = new ClinicPatternSqlServices(_context, _config);
            _ssAdHoc = new AdHocClinicSqlServices(_context, _config);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _audit = new AuditSqlServices(_config);
        }
        public List<StaffMember> staffMemberList { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        
        public string Message;
        public bool isSuccess;
        
        public void OnGet()
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }                
                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();
                _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic Setup", "");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        
        public void OnPost(DateTime dStartDate, DateTime dEndDate, int startHr, int startMin, string clinicianID, string clinicID, 
            int dayNum, int weekNum, int duration, int numSlots,
            string monthstring, bool? isNewStandard, bool? isModifyStandard, bool? isNewAdHoc, bool? isModifyAdHoc)
        {
            try
            {
                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();
                string username = User.Identity.Name;
                string sStaffCode = _staffData.GetStaffDetailsByUsername(username).STAFF_CODE;

                if (dStartDate == DateTime.Parse("0001-01-01"))
                {
                    dStartDate = DateTime.Today;
                }

                if (isNewStandard.GetValueOrDefault())
                {
                    if(clinicianID == null || clinicID == null || startHr == 0 || dayNum == 0 || 
                        weekNum == 0 || duration == 0 || numSlots == 0)
                    {
                        Message = "Missing data. Please try again.";
                        isSuccess = false;
                    }
                    else
                    {
                        isSuccess = true;
                        
                        if(monthstring == null)
                        {
                            monthstring = "123456789abc";
                        }
                        
                        _ssPat.SaveClinicPattern(clinicianID, clinicID, dayNum, weekNum, monthstring, numSlots, duration, startHr, startMin, dStartDate, dEndDate, username);
                    }
                }

                if (isModifyStandard.GetValueOrDefault())
                {
                    Response.Redirect("ClinicPatternList?staffCode=" + clinicianID);
                }

                if (isNewAdHoc.GetValueOrDefault())
                {
                    if (clinicianID == null || clinicID == null || startHr == 0 || duration == 0 || numSlots == 0)
                    {
                        Message = "Missing data. Please try again.";
                        isSuccess = false;
                    }
                    else
                    {
                        isSuccess = true;

                        _ssAdHoc.SaveAdHocClinic(clinicianID, clinicID, numSlots, duration, startHr, startMin, dStartDate, username);
                    }
                }

                if (isModifyAdHoc.GetValueOrDefault())
                {
                    Response.Redirect("AdHocClinicList?staffCode=" + clinicianID);
                }

                if (isSuccess)
                {
                    Response.Redirect("Index");
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }        
    }
}
