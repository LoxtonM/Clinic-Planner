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
        DataConnections dc;

        public ClinicSetupModel(DataContext context)
        {
            _context = context;
            dc = new DataConnections(_context);
        }
        public List<StaffMember> staffMemberList { get; set; }
        public List<ClinicVenue> clinicVenueList { get; set; }
        
        public string Message;
        public bool isSuccess;
        
        public void OnGet()
        {
            try
            {
                staffMemberList = dc.GetStaffMemberList();
                clinicVenueList = dc.GetVenueList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
        
        public void OnPost(DateTime dStartDate, DateTime dEndDate, int iStartHr, int iStartMin, string sClinicianID, string sClinicID, 
            int iDayNum, int iWeekNum, int iDuration, int iNumSlots,
            string sMonthString, bool? isNewStandard, bool? isModifyStandard, bool? isNewAdHoc, bool? isModifyAdHoc)
        {
            try
            {

                staffMemberList = dc.GetStaffMemberList();
                clinicVenueList = dc.GetVenueList();
                string sUsername = "LoxM";

                if(dStartDate == DateTime.Parse("0001-01-01"))
                {
                    dStartDate = DateTime.Today;
                }

                if (isNewStandard.GetValueOrDefault())
                {
                    if(sClinicianID == null || sClinicID == null || iStartHr == 0 || iDayNum == 0 || 
                        iWeekNum == 0 || iDuration == 0 || iNumSlots == 0)
                    {
                        Message = "Missing data. Please try again.";
                        isSuccess = false;
                    }
                    else
                    {
                        isSuccess = true;
                        
                        if(sMonthString == null)
                        {
                            sMonthString = "123456789abc";
                        }

                        SetupStandardClinicPattern(dStartDate, dEndDate, iStartHr, iStartMin, sClinicianID, sClinicID, iDayNum, iWeekNum, 
                            iDuration, iNumSlots, sMonthString, sUsername);
                    }

                    
                }

                if (isModifyStandard.GetValueOrDefault())
                {
                    Response.Redirect("WIP");
                }

                if (isNewAdHoc.GetValueOrDefault())
                {
                    if (sClinicianID == null || sClinicID == null || iStartHr == 0 || iDuration == 0 || iNumSlots == 0)
                    {
                        Message = "Missing data. Please try again.";
                        isSuccess = false;
                    }
                    else
                    {
                        isSuccess = true;
                        
                        SetupAdHocClinic(dStartDate, iStartHr, iStartMin, sClinicianID, sClinicID, iDuration, iNumSlots, sUsername);
                    }
                }

                if (isModifyAdHoc.GetValueOrDefault())
                {
                    Response.Redirect("WIP");
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

        public void SetupStandardClinicPattern(DateTime dStartDate, DateTime dEndDate, int iStartHr, int iStartMin, string sClinicianID, string sClinicID,
            int iDayNum, int iWeekNum, int iDuration, int iNumSlots, string sMonthString, string sUsername)
        {
            SetupClinicSlots(dStartDate, dEndDate, iStartHr, iStartMin, sClinicianID, sClinicID, iDayNum, iWeekNum, iDuration,
                    sMonthString, sUsername);
        }

        public void SetupAdHocClinic(DateTime dStartDate, int iStartHr, int iStartMin, string sClinicianID, string sClinicID,
            int iDuration, int iNumSlots, string sUsername)
        {
            //do stuff
        }


        public void SetupClinicSlots(DateTime dStartDate, DateTime dEndDate, int iStartHr, int iStartMin, string sClinicianID, string sClinicID, int iDayNum, int iWeekNum, int iDuration,
            string sMonthString, string sUsername)
        {
            int iToday = (int)DateTime.Today.DayOfWeek;
            //Monday = 1, Tuesday = 2, Wednesday = 3, Thursday = 4, Friday = 5

        }
    }
}
