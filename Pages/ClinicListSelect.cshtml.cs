using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;
using CPTest.Connections;
using CPTest.Data;
using CPTest.Document;
using ClinicalXPDataConnections.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicListSelectModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly DocumentContext _documentContext;
        private readonly IConfiguration _config;
        private readonly IStaffData _staffData;
        private readonly IAuditSqlServices _audit;
        private readonly IAppointmentData _appt;
        private readonly IDocumentController _doc;

        public ClinicListSelectModel(ClinicalContext context, CPXContext cpxContext, DocumentContext documentContext, IConfiguration config)
        {
            _context = context;
            _cpxContext = cpxContext;
            _documentContext = documentContext;
            _config = config;
            _staffData = new StaffData(_context);
            _appt = new AppointmentData(_context);
            _doc = new DocumentController(_context, _cpxContext, _documentContext);
            _audit = new AuditSqlServices(_config);
        }

        public List<Appointment> apptList { get; set; }

        public string? wcDateStr;
        public string? clinicianSel;
        public string? clinicSel;

        public void OnGet(string? wcDateString, string? clinicianid, string? clinicid, string? clinicDateString)
        {
            try
            {
                wcDateStr = wcDateString;
                clinicianSel = clinicianid;
                clinicSel = clinicid;

                List<Appointment> appts = new List<Appointment>();
                appts = _appt.GetAppointments(DateTime.Parse(wcDateString), DateTime.Parse(wcDateString).AddDays(7), clinicianid, clinicid).Distinct().ToList();
                apptList = new List<Appointment>();

                if (clinicDateString != null)
                {
                    DateTime clinicDate = DateTime.Parse(clinicDateString);
                    Appointment appt = appts.First(a => a.BOOKED_DATE == clinicDate);
                    int refid = appt.RefID;
                    if (_doc.ClinicList(refid, User.Identity.Name) == 1)
                    { 
                        Response.Redirect(@Url.Content($"~/cliniclist-{User.Identity.Name}.pdf"));
                        IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                        _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic List Print", "RefID=" + refid.ToString(), _ip.GetIPAddress());
                    }
                    else
                    {
                        string message = "Something went wrong and the letter didn't print for some reason.";
                        Response.Redirect("Error?sError=" + message);
                    }
                }
                else
                {
                    foreach (var item in appts)
                    {
                        if (apptList.Where(a => a.BOOKED_DATE == item.BOOKED_DATE).Count() == 0)
                        {
                            apptList.Add(item);
                        }
                    }
                }

                apptList = apptList.OrderBy(a => a.BOOKED_DATE).ToList();
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int refid)
        {
            try
            {
                Appointment appt = _appt.GetAppointmentDetails(refid);

                string clinicianid = appt.STAFF_CODE_1;
                string clinicid = appt.FACILITY;

                apptList = _appt.GetAppointments(DateTime.Now, DateTime.Now.AddDays(365), clinicianid, clinicid);

                string returnUrl = "Index?wcDt=" + wcDateStr;
                if (clinicianSel != null) { returnUrl = returnUrl + $"&clinician={clinicianSel}"; }
                if (clinicSel != null) { returnUrl = returnUrl + $"&clinic={clinicSel}"; }

                if (_doc.ClinicList(refid, User.Identity.Name) == 1)
                {
                    Response.Redirect(@Url.Content($"~/cliniclist-{User.Identity.Name}.pdf"));
                    IPAddressFinder _ip = new IPAddressFinder(HttpContext);
                    _audit.CreateAudit(_staffData.GetStaffDetailsByUsername(User.Identity.Name).STAFF_CODE, "Clinic List Print", "RefID=" + refid.ToString(), _ip.GetIPAddress());
                }
                else
                {
                    string message = "Something went wrong and the letter didn't print for some reason.";
                    Response.Redirect("Error?sError=" + message);
                }

                //Response.Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
