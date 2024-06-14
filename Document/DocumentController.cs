using CPTest.Connections;
using CPTest.Data;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using Spire.Pdf;
using System.Net.Mail; //must use an older version as 10.2 doesn't work with .NET 6 anymore!


namespace CPTest.Document
{
    public class DocumentController
    {
        private readonly DataContext _context;
        private readonly IPatientData _patient;
        private readonly IAppointmentData _appointment;
        private readonly IStaffData _staff;
        private readonly IClinicVenueData _clinic;
        private readonly IReferralData _referral;
        private readonly IConstantData _constant;
        private readonly IExternalClinicianData _externalClinician;
        private readonly IDocumentsContentData _docContent;


        public DocumentController(DataContext context)
        {
            _context = context;
            _patient = new PatientData(_context);
            _appointment = new AppointmentData(_context);
            _staff = new StaffData(_context);
            _clinic = new ClinicVenueData(_context);
            _referral = new ReferralData(_context);
            _constant = new ConstantData(_context);
            _externalClinician = new ExternalClinicianData(_context);
            _docContent = new DocumentsContentData(_context);
        }
        public int ClinicLetter(int refID, bool isEmailOnly)
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
                int totalLength = 200;

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
                tf.DrawString("Consultant: " + referral.LeadClinician, font, XBrushes.Black, new XRect(50, 100, 500, 10));
                tf.DrawString("GC: " + referral.GC, font, XBrushes.Black, new XRect(50, 115, 500, 10));
                tf.DrawString("NHS No: " + pat.SOCIAL_SECURITY, font, XBrushes.Black, new XRect(50, 130, 500, 10));
                tf.DrawString("Please quote our reference number on all correspondence: " + pat.CGU_No, font, XBrushes.Black, new XRect(50, 145, 500, 10));
                tf.DrawString(DateTime.Today.ToString("dd MMMM yyyy"), font, XBrushes.Black, new XRect(50, 160, 500, 10)); //today's date

                tf.Alignment = XParagraphAlignment.Right;
                //Our address and contact details
                tf.DrawString("Clinical Genetics Unit", font, XBrushes.Black, new XRect(-20, 150, page.Width, 200));
                tf.DrawString("Tel: " + _constant.GetConstantValue("MainCGUPhoneNumber").Trim(), fontBold, XBrushes.Black, new XRect(-20, 165, page.Width, 10));
                tf.DrawString("Email: " + _constant.GetConstantValue("MainCGUEmail").Trim(), font, XBrushes.Black, new XRect(-20, 180, page.Width, 10));

                //patient's address
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(pat.PtLetterAddressee, font, XBrushes.Black, new XRect(50, totalLength, 500, 10));
                totalLength = totalLength + 15;

                string salutation = "";
                string openingBlurb = "";
                if (pat.DOB.AddYears(16) > DateTime.Now)
                {
                    salutation = "Parent/Guardian of " + pat.SALUTATION;
                    openingBlurb = docContent.Para1 + extClinician.TITLE + " " + extClinician.FIRST_NAME + " " + extClinician.NAME + ", " + docContent.Para11;
                }
                else
                {
                    salutation = pat.SALUTATION;
                    openingBlurb = docContent.Para2 + extClinician.TITLE + " " + extClinician.FIRST_NAME + " " + extClinician.NAME + ", " + docContent.Para12;
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
                totalLength = totalLength + address.Length * 2;

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
                    tf.DrawString(clinic.NOTES, font, XBrushes.Black, new XRect(50, totalLength, 500, clinic.NOTES.Length / 4));
                    totalLength = totalLength + clinic.NOTES.Length / 4;
                }

                tf.DrawString(docContent.Para3, fontBold, XBrushes.Black, new XRect(50, totalLength, 500, 10)); //"What if I can't attend?"
                totalLength = totalLength + 20;

                tf.DrawString(docContent.Para8, font, XBrushes.Black, new XRect(50, totalLength, 500, 75)); //"If you can't attend, please tell someone... etc"

                totalLength = totalLength + 75;

                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(docContent.Para9, fontBoldUnderlined, XBrushes.Black, new XRect(50, totalLength, 500, 40)); //"It is our policy not to offer another..."

                totalLength = totalLength + 40;
                totalLength = totalLength + 15;

                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Yours sincerely,", font, XBrushes.Black, new XRect(50, totalLength, 500, 20));
                totalLength = totalLength + 50;

                tf.DrawString("Clinical Genetics Booking Centre", font, XBrushes.Black, new XRect(50, totalLength, 500, 20));


                document.Save("letter.pdf");

                if (!isEmailOnly)
                {
                    Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument("letter.pdf");
                    doc.Print();
                }
                else
                {

                }

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public int ClinicList(string clinicID, DateTime clinicDate, bool isEmailOnly)
        {
            try
            {


                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
