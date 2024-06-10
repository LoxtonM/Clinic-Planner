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
        //private readonly IClinicSlotsCreator _csc;
        private readonly IClinicPatternSqlServices _ssPat;
        private readonly IAdHocClinicSqlServices _ssAdHoc;

        public ClinicSetupModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            //_csc = new ClinicSlotsCreator(_context, _config);
            _ssPat = new ClinicPatternSqlServices(_context, _config);
            _ssAdHoc = new AdHocClinicSqlServices(_context, _config);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
        }
        public List<StaffMember> staffMemberList { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        
        public string Message;
        public bool isSuccess;
        
        public void OnGet()
        {
            try
            {
                staffMemberList = _staffData.GetStaffMemberList();
                clinicVenueList = _clinicVenueData.GetVenueList();
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
                string sStaffCode = _staffData.GetStaffDetailsByUsername("mnln").STAFF_CODE;
                string username = "mnln";

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

        //public void SetupStandardClinicPattern(DateTime dStartDate, DateTime dEndDate, int startHr, int startMin, string clinicianID, string clinicID,
        //    int dayNum, int weekNum, int duration, int numSlots, string monthstring, string sUsername)
       // {
       //     SetupClinicSlots(dStartDate, dEndDate, startHr, startMin, clinicianID, clinicID, dayNum, weekNum, duration,
       //             monthstring, sUsername);
       // }

        //public void SetupAdHocClinic(DateTime dStartDate, int startHr, int startMin, string clinicianID, string clinicID,
        //    int duration, int numSlots, string sUsername)
       // {
       //     //do stuff
       // }


       // public void SetupClinicSlots(DateTime dStartDate, DateTime dEndDate, int startHr, int startMin, string clinicianID, string clinicID, int dayNum, int weekNum, int duration,
       //     string monthstring, string sUsername)
       // {
        //    int iToday = (int)DateTime.Today.DayOfWeek;
        //    //Monday = 1, Tuesday = 2, Wednesday = 3, Thursday = 4, Frday = 5

       // }
    }
}
