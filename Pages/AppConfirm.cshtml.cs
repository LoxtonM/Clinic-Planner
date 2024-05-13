using CPTest.Connections;
using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class AppConfirmModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IPatientData _patientData;
        private readonly IStaffData _staffData;
        private readonly IClinicVenueData _clinicVenueData;
        private readonly IReferralData _referralData;
        private readonly IAppTypeData _appTypeData;        
        private readonly SqlServices _ss;

        public AppConfirmModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;            
            _ss = new SqlServices(_config);
            _patientData = new PatientData(_context);
            _staffData = new StaffData(_context);
            _clinicVenueData = new ClinicVenueData(_context);
            _referralData = new ReferralData(_context);
            _appTypeData = new AppTypeData(_context);
        }

        public Patient? patient { get; set; }
        public StaffMember? staffMember { get; set; }
        public ClinicVenue? clinicVenue { get; set; }
        public List<Referral> linkedRefList { get; set; }
        public List<AppType> appTypeList { get; set; }

        public DateTime appDate;
        public DateTime appTime;
        public int appDur;
        public string? appDateString;
        public string? appTimeString;
        public string? appTypeDef;
        
        public void OnGet(string intIDString, string clin, string ven, string dat, string tim, string dur, string instructions)
        {
            try
            {
                int intID = Int32.Parse(intIDString);
                int mpi = 0;

                patient = _patientData.GetPatientDetailsByIntID(intID);

                if (patient == null)
                {
                    Response.Redirect("PatientNotFound?intID=" + intID.ToString() + "&clinicianID=" + clin + "&clinicID=" + ven, true);
                }
                else
                {
                    mpi = patient.MPI;
                }

                appTypeList = _appTypeData.GetAppTypeList();
                staffMember = _staffData.GetStaffDetails(clin);

                clinicVenue = _clinicVenueData.GetVenueDetails(ven);

                linkedRefList = _referralData.GetReferralsList(mpi);

                appDateString = dat;
                appTimeString = tim;

                appDate = DateTime.Parse(dat);
                appTime = DateTime.Parse("1899-12-30 " + tim);
                appDur = Int32.Parse(dur);

                if (staffMember.CLINIC_SCHEDULER_GROUPS == "GC")
                {
                    appTypeDef = "GC Only Appt";
                }
                else
                {
                    appTypeDef = "Cons. Appt";
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int mpi, int refID, string clin, string ven, DateTime dat, string tim, int dur, string instructions, string type)
        {
            try
            {
                string staffCode;

                patient = _patientData.GetPatientDetails(mpi);
                appTypeList = _appTypeData.GetAppTypeList();
                staffMember = _staffData.GetStaffDetails(clin);
                staffCode = _staffData.GetStaffDetailsByUsername("mnln").STAFF_CODE; //placeholder - will replace when login screen available                
                
                clinicVenue = _clinicVenueData.GetVenueDetails(ven);

                linkedRefList = _referralData.GetReferralsList(mpi);

                _ss.CreateAppointment(dat, tim, clin, null, null, ven, refID, mpi, type, dur, staffCode, instructions);
                
                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
