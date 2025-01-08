using CPTest.Connections;
using CPTest.Data;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;
using Spire.Pdf;



namespace CPTest.Document
{
    interface IDocumentController
    {
        public int ClinicLetter(int refID, string username, bool isEmailOnly);
        public int ClinicList(int refID, string username);
    }
    public class DocumentController : IDocumentController
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        private readonly DocumentContext _documentContext;
        private readonly IPatientData _patient;
        private readonly IAppointmentData _appointment;
        private readonly IStaffData _staff;
        private readonly IClinicVenueData _clinic;
        private readonly IReferralData _referral;
        private readonly IConstantsData _constant;
        private readonly IExternalClinicianData _externalClinician;
        private readonly IDocumentsContentData _docContent;
        private readonly IClinicDetailsData _clinicDetails;


        public DocumentController(ClinicalContext context, CPXContext cpxContext, DocumentContext documentContext)
        {
            _context = context;
            _cpxContext = cpxContext;
            _documentContext = documentContext;
            _patient = new PatientData(_context);
            _appointment = new AppointmentData(_context);
            _staff = new StaffData(_context);
            _clinic = new ClinicVenueData(_context);
            _referral = new ReferralData(_context);
            _constant = new ConstantsData(_documentContext);
            _externalClinician = new ExternalClinicianData(_context);
            _docContent = new DocumentsContentData(_documentContext);
            _clinicDetails = new ClinicDetailsData(_cpxContext);
        }
        public int ClinicLetter(int refID, string username, bool isEmailOnly)
        {
            try
            {
                var appt = _appointment.GetAppointmentDetails(refID);
                var pat = _patient.GetPatientDetails(appt.MPI);
                var clinician = _staff.GetStaffDetails(appt.STAFF_CODE_1);
                var clinic = _clinic.GetVenueDetails(appt.FACILITY);
                var referral = _referral.GetReferralDetails(appt.ReferralRefID);
                var extClinician = _externalClinician.GetClinicianDetails(referral.ReferrerCode);
                if (extClinician == null)
                {
                    extClinician = _externalClinician.GetClinicianDetails("Unknown"); //because of course there are nulls.
                }
                var docContent = _docContent.GetDocumentContent(164);
                int totalLength = 50;

                PdfSharpCore.Pdf.PdfDocument document = new PdfSharpCore.Pdf.PdfDocument();
                PdfPage page = document.AddPage();

                XGraphics gfx = XGraphics.FromPdfPage(page);
                var tf = new XTextFormatter(gfx);
                //set the fonts used for the letters
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);
                XFont fontSmall = new XFont("Arial", 10, XFontStyle.Regular);
                XFont fontBold = new XFont("Arial", 12, XFontStyle.Bold);
                XFont fontBoldUnderlined = new XFont("Arial", 12, XFontStyle.Underline | XFontStyle.Bold);
                //Load the image for the letter head            
                XImage image = XImage.FromFile(@"wwwroot\Images\Letterhead.jpg");
                gfx.DrawImage(image, 350, 20, image.PixelWidth / 2, image.PixelHeight / 2);
                tf.DrawString("Consultant: " + referral.LeadClinician, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;
                tf.DrawString("GC: " + referral.GC, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;
                tf.DrawString("NHS No: " + pat.SOCIAL_SECURITY, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;
                tf.DrawString("Please quote our reference number on all correspondence: " + pat.CGU_No, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;
                tf.DrawString(DateTime.Today.ToString("dd MMMM yyyy"), font, XBrushes.Black, new XRect(50, totalLength, 500, 10)); //today's date
                totalLength = totalLength + 30;

                tf.Alignment = XParagraphAlignment.Right;
                //Our address and contact details
                tf.DrawString("Clinical Genetics Unit", font, XBrushes.Black, new XRect(-20, 150, page.Width, 200));
                tf.DrawString("Tel: " + _constant.GetConstant("MainCGUPhoneNumber", 1).Trim(), fontBold, XBrushes.Black, new XRect(-20, 165, page.Width, 10));
                tf.DrawString("Email: " + _constant.GetConstant("MainCGUEmail", 1).Trim(), font, XBrushes.Black, new XRect(-20, 180, page.Width, 10));

                //patient's address
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(pat.PtLetterAddressee, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;

                string salutation = "";
                string openingBlurb = "";
                if (pat.DOB.GetValueOrDefault().AddYears(16) > DateTime.Now)
                {
                    salutation = "Parent/Guardian of " + pat.SALUTATION;
                    openingBlurb = docContent.Para1 + extClinician.TITLE + " " + extClinician.FIRST_NAME + " " + extClinician.NAME + ", " + docContent.Para11;
                }
                else
                {
                    salutation = pat.SALUTATION;
                    openingBlurb = docContent.Para2 + " " + extClinician.TITLE + " " + extClinician.FIRST_NAME + " " + extClinician.NAME + ", " + docContent.Para12;
                }

                string address = pat.ADDRESS1 + Environment.NewLine;
                if (pat.ADDRESS2 != null) //this is sometimes null
                {
                    address = address + pat.ADDRESS2 + Environment.NewLine;
                }
                address = address + pat.ADDRESS3 + Environment.NewLine;
                address = address + pat.ADDRESS4 + Environment.NewLine;
                address = address + pat.POSTCODE;

                tf.DrawString(address, font, XBrushes.Black, new XRect(50, totalLength, 250, address.Length * 2));
                totalLength = totalLength + address.Length;

                totalLength = totalLength + 20; //because we can't get the address to work just by its length

                //Date letter created

                tf.DrawString("Dear " + salutation, font, XBrushes.Black, new XRect(50, totalLength, 500, 10)); //salutation
                totalLength = totalLength + 15;

                tf.DrawString(openingBlurb + " " + clinician.NAME + ", " + clinician.POSITION + " at:", font, XBrushes.Black, new XRect(50, totalLength, 500, 50)); //Following the referral, etc...

                totalLength = totalLength + 50;

                tf.Alignment = XParagraphAlignment.Center;

                tf.DrawString(clinic.LOCATION.Replace("  ", System.Environment.NewLine), fontBold, XBrushes.Black, new XRect(225, totalLength, 150, clinic.LOCATION.Length));
                totalLength = totalLength + clinic.LOCATION.Length; //Clinic venue location

                tf.DrawString("on", font, XBrushes.Black, new XRect(50, totalLength, 500, 20));

                totalLength = totalLength + 20;

                tf.DrawString(appt.BOOKED_DATE.Value.ToString("dddd, dd MMMM yyyy") + " at " + appt.BOOKED_TIME.Value.ToString("HH:mm"),
                    fontBold, XBrushes.Black, new XRect(50, totalLength, 500, 20)); //date and time

                totalLength = totalLength + 20;

                tf.DrawString("The appointment usually lasts about " + appt.Duration + " minutes.", font, XBrushes.Black, new XRect(50, totalLength, 500, 10));

                totalLength = totalLength + 20;

                tf.Alignment = XParagraphAlignment.Left;

                if (clinic.NOTES != null && clinic.NOTES != "") //special instructions that may or mat not be present
                {
                    tf.DrawString(clinic.NOTES, font, XBrushes.Black, new XRect(50, totalLength, 500, clinic.NOTES.Length / 2));
                    totalLength = totalLength + clinic.NOTES.Length / 2;
                }

                tf.DrawString(docContent.Para3, fontBold, XBrushes.Black, new XRect(50, totalLength, 500, 10)); //"What if I can't attend?"
                totalLength = totalLength + 20;

                tf.DrawString(docContent.Para8, font, XBrushes.Black, new XRect(50, totalLength, 500, 75)); //"If you can't attend, please tell someone... etc"

                totalLength = totalLength + 75;

                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(docContent.Para9, fontBold, XBrushes.Black, new XRect(50, totalLength, 500, 40)); //"It is our policy not to offer another..."

                totalLength = totalLength + 40;
                totalLength = totalLength + 15;

                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Yours sincerely,", font, XBrushes.Black, new XRect(50, totalLength, 500, 20));
                totalLength = totalLength + 50;

                tf.DrawString("Clinical Genetics Booking Centre", font, XBrushes.Black, new XRect(50, totalLength, 500, 20));
                //totalLength = totalLength + 50;
                //tf.DrawString(docContent.DocCode, font, XBrushes.Black, new XRect(500, totalLength, 500, 20));

                if (File.Exists($"wwwroot/letter-{username}.pdf"))
                {
                    File.Delete($"wwwroot/letter-{username}.pdf");
                }
                document.Save($"wwwroot/letter-{username}.pdf");

                if (!isEmailOnly)
                {
                    string printerName = _constant.GetConstant("SynertecPrinterName", 1).Trim();
                    
                    using (Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument())
                    {
                        pdf.LoadFromFile($"wwwroot/letter-{username}.pdf");
                        pdf.PrintSettings.PrinterName = printerName;
                        pdf.Print();
                    }
                }
                                
                return 1;
            }
            catch (Exception ex)
            {                
                return 0;
            }
        }


        public int ClinicList(int refID, string username)
        {
            try
            {
                PdfSharpCore.Pdf.PdfDocument document = new PdfSharpCore.Pdf.PdfDocument();
                PdfPage page = document.AddPage();
                PdfPage page2 = document.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                page2.Orientation = PdfSharpCore.PageOrientation.Landscape;

                XGraphics gfx = XGraphics.FromPdfPage(page);
                XGraphics gfx2 = XGraphics.FromPdfPage(page2);
                var tf = new XTextFormatter(gfx);
                var tf2 = new XTextFormatter(gfx2);
                //set the fonts used for the letters
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);
                XFont fontSmall = new XFont("Arial", 10, XFontStyle.Regular);
                XFont fontBold = new XFont("Arial", 12, XFontStyle.Bold);
                XFont fontBoldUnderlined = new XFont("Arial", 12, XFontStyle.Underline | XFontStyle.Bold);
                //Load the image for the letter head            
                XImage image = XImage.FromFile(@"wwwroot\Images\Letterhead.jpg");
                tf.Alignment = XParagraphAlignment.Right;
                gfx.DrawImage(image, 575, 20, image.PixelWidth / 2, image.PixelHeight / 2);
                tf.DrawString("WEST MIDLANDS REGIONAL CLINICAL GENETICS SERVICE", fontBold, XBrushes.Black, new XRect(325, 130, 500, 10));

                int totalLength = 50;
                int totalLength2 = 50;
                tf.Alignment = XParagraphAlignment.Left;

                var appt = _appointment.GetAppointmentDetails(refID);
                var clinician = _staff.GetStaffDetails(appt.STAFF_CODE_1);
                var clinic = _clinic.GetVenueDetails(appt.FACILITY);
                var clinicList = _appointment.GetAppointmentsForADay(appt.BOOKED_DATE.GetValueOrDefault(), clinician.STAFF_CODE, clinic.FACILITY);
                var clinicDetails = _clinicDetails.GetClinicDetails(clinic.FACILITY);

                //tf.DrawString(clinician.NAME, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                //totalLength = totalLength + 15;
                //tf.DrawString(clinician.POSITION, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                //totalLength = totalLength + 40;

                string addressee = clinicDetails.Addressee;
                if(clinicDetails.Position != null)
                {
                    addressee = addressee + ", " + clinicDetails.Position + System.Environment.NewLine;
                }
                addressee = addressee + clinicDetails.A_Address + System.Environment.NewLine;
                addressee = addressee + clinicDetails.A_Town + System.Environment.NewLine;
                addressee = addressee + clinicDetails.A_PostCode;
                tf.DrawString(addressee, font, XBrushes.Black, new XRect(50, totalLength, 500, 100));
                totalLength = totalLength + 100;

                tf.DrawString(clinicDetails.A_Salutation + ", ", font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;
                string clinicDetail = "RE: " + clinic.NAME + " - " + clinicDetails.ClinicSite + " on " + appt.BOOKED_DATE.Value.ToString("dddd MMMM yyyy");
                clinicDetail = clinicDetail + " To be held by " + clinician.NAME + ", " + clinician.POSITION;
                tf.DrawString(clinicDetail, font, XBrushes.Black, new XRect(50, totalLength, 800, 40));
                totalLength = totalLength + 25;
                tf.DrawString(clinicDetails.Preamble + ", ", font, XBrushes.Black, new XRect(50, totalLength, 800, 100));


                totalLength = totalLength + 40; //appointments list

                tf.DrawString("Time", fontBold, XBrushes.Black, new XRect(20, totalLength, 500, 10));
                tf.DrawString("Patient", fontBold, XBrushes.Black, new XRect(70, totalLength, 500, 10));
                tf.DrawString("Refs", fontBold, XBrushes.Black, new XRect(250, totalLength, 500, 10));
                tf.DrawString("GP", fontBold, XBrushes.Black, new XRect(400, totalLength, 500, 10));
                tf.DrawString("Referred by", fontBold, XBrushes.Black, new XRect(500, totalLength, 500, 10));
                tf.DrawString("Instructions (post clinic)", fontBold, XBrushes.Black, new XRect(600, totalLength, 500, 10));

                foreach (var item in clinicList)
                {
                    var pat = _patient.GetPatientDetails(item.MPI);
                    string address = pat.ADDRESS1 + ", ";
                    if (pat.ADDRESS2 != null)
                    {
                        address = address + pat.ADDRESS2 + ", ";
                    }
                    address = address + pat.ADDRESS3 + ", ";
                    address = address + pat.ADDRESS4 + System.Environment.NewLine;
                    address = address + pat.POSTCODE;
                    string patient = pat.FIRSTNAME + " " + pat.LASTNAME + ", " + pat.DOB.Value.ToString("dd/MM/yyyy") + System.Environment.NewLine + pat.POSTCODE;
                    string ourrefs = "Our ref: " + pat.CGU_No + System.Environment.NewLine + "NHS No: " + pat.SOCIAL_SECURITY;
                    string gpName = "Unknown";
                    if (pat.GP != null) //because there are ALWAYS nulls somewhere!
                    {
                        gpName = pat.GP + System.Environment.NewLine + pat.GP_Facility; ;
                    }
                    var referral = _referral.GetReferralDetails(item.ReferralRefID);
                    string referrerName = "Unknown";
                    if (referral.ReferringClinician != null)
                    {
                        referrerName = referral.ReferringClinician + System.Environment.NewLine + referral.ReferringFacility;
                    }

                    if (totalLength < 350)
                    {
                        totalLength = totalLength + 30;
                        tf.DrawString(item.BOOKED_TIME.Value.ToString("HH:mm"), font, XBrushes.Black, new XRect(20, totalLength, 500, 10));                        
                        tf.DrawString(patient, font, XBrushes.Black, new XRect(70, totalLength, 200, 80)); //patient demographics                        
                        tf.DrawString(ourrefs, font, XBrushes.Black, new XRect(250, totalLength, 150, 30)); //CGU and NHS numbers                        
                        tf.DrawString(gpName, font, XBrushes.Black, new XRect(400, totalLength, 100, 30)); //GP                        
                        tf.DrawString(referrerName, font, XBrushes.Black, new XRect(500, totalLength, 100, 30));
                    }
                    else
                    {
                        totalLength2 = totalLength2 + 30;
                        tf2.DrawString(item.BOOKED_TIME.Value.ToString("HH:mm"), font, XBrushes.Black, new XRect(20, totalLength2, 500, 10));                        
                        tf2.DrawString(patient, font, XBrushes.Black, new XRect(70, totalLength2, 200, 80)); //patient demographics
                        tf2.DrawString(ourrefs, font, XBrushes.Black, new XRect(250, totalLength2, 150, 30)); //CGU and NHS numbers
                        tf2.DrawString(gpName, font, XBrushes.Black, new XRect(400, totalLength2, 100, 30)); //GP                        
                        tf2.DrawString(referrerName, font, XBrushes.Black, new XRect(500, totalLength2, 100, 30));
                    }
                }
                string postLude = "";
                if (clinicDetails.Postlude != null)
                {
                    postLude = clinicDetails.Postlude;
                }

                if (totalLength <= 400)
                {
                    totalLength = 450;
                    tf.DrawString(postLude, font, XBrushes.Black, new XRect(50, totalLength, 500, 100));
                    totalLength = totalLength + 20;
                    if (clinicDetails.Secretary != null)
                    {
                        tf.DrawString("Yours sincerely", font, XBrushes.Black, new XRect(50, totalLength, 100, 30));
                        totalLength = totalLength + 50;                    
                        tf.DrawString(clinicDetails.Secretary, font, XBrushes.Black, new XRect(50, totalLength, 100, 10));
                        totalLength = totalLength + 15;
                        tf.DrawString("Secretary to " + clinician.NAME + ", " + clinician.POSITION, font, XBrushes.Black, new XRect(50, totalLength, 500, 30));
                        totalLength = totalLength + 15;
                    }
                    //copies to etc
                }
                else
                {
                    totalLength2 = 450;
                    tf2.DrawString(postLude, font, XBrushes.Black, new XRect(50, totalLength2, 500, 100));
                    totalLength2 = totalLength2 + 20;
                    if (clinicDetails.Secretary != null)
                    {
                        tf2.DrawString("Yours sincerely", font, XBrushes.Black, new XRect(50, totalLength2, 100, 30));
                        totalLength2 = totalLength2 + 50;                    
                        tf2.DrawString(clinicDetails.Secretary, font, XBrushes.Black, new XRect(50, totalLength2, 100, 10));
                        totalLength2 = totalLength2 + 15;
                        tf2.DrawString("Secretary to " + clinician.NAME + ", " + clinician.POSITION, font, XBrushes.Black, new XRect(50, totalLength2, 500, 30));
                        totalLength2 = totalLength2 + 15;
                    }
                    //copies to etc
                }

                if(totalLength2 == 50)
                {
                    document.Pages.Remove(page2);
                }

                if (File.Exists($"wwwroot/cliniclist-{username}.pdf"))
                {
                    File.Delete($"wwwroot/cliniclist-{username}.pdf");
                }
                document.Save($"wwwroot/cliniclist-{username}.pdf");

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
