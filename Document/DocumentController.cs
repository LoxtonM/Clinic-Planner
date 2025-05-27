using CPTest.Connections;
using CPTest.Data;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Meta;

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

                

                //string contactData = "Clinical Genetics Unit" + Environment.NewLine;
                //contactData = contactData + "Tel: " + _constant.GetConstant("MainCGUPhoneNumber", 1).Trim() + Environment.NewLine;
                //contactData = contactData + "Email: " + _constant.GetConstant("MainCGUEmail", 1).Trim() + Environment.NewLine;

                MigraDoc.DocumentObjectModel.Document document = new MigraDoc.DocumentObjectModel.Document();
                                
                Section section = document.AddSection();
                section.PageSetup.LeftMargin = "1.5cm";
                section.PageSetup.RightMargin = "1.5cm";
                section.PageSetup.TopMargin = "1cm";
                section.PageSetup.BottomMargin = "1cm";

                Paragraph logo = section.AddParagraph();

                MigraDoc.DocumentObjectModel.Shapes.Image imgLogo = logo.AddImage(@"wwwroot\Images\Letterhead.jpg"); //add the logo
                imgLogo.ScaleWidth = new Unit(0.5, UnitType.Point);
                imgLogo.ScaleHeight = new Unit(0.5, UnitType.Point);
                logo.Format.Alignment = ParagraphAlignment.Right;

                Paragraph spacer = section.AddParagraph();
                spacer = section.AddParagraph();

                MigraDoc.DocumentObjectModel.Tables.Table table = section.AddTable(); //tables are used to get stuff to appear side by side
                MigraDoc.DocumentObjectModel.Tables.Column contactInfo = table.AddColumn();
                contactInfo.Format.Alignment = ParagraphAlignment.Left;
                MigraDoc.DocumentObjectModel.Tables.Column ourAddressInfo = table.AddColumn();
                ourAddressInfo.Format.Alignment = ParagraphAlignment.Right;

                table.Rows.Height = 20;
                table.Columns.Width = 250;
                MigraDoc.DocumentObjectModel.Tables.Row row1 = table.AddRow();
                row1.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Top;
                
                string details = ""; //contactData + Environment.NewLine;
                details = details + "Consultant: " + referral.LeadClinician + Environment.NewLine;
                details = details + "Genetic Counsellor: " + referral.GC + Environment.NewLine;
                details = details + "NHS number: " + pat.SOCIAL_SECURITY + Environment.NewLine;

                string quoteRef = "Please quote our reference on all correspondence: " + pat.CGU_No + Environment.NewLine;

                row1.Cells[0].AddParagraph(details);

                row1.Cells[1].AddParagraph("Clinical Genetics Unit" + Environment.NewLine + "Tel: " + _constant.GetConstant("MainCGUPhoneNumber", 1) + Environment.NewLine + "Email: " + _constant.GetConstant("MainCGUEmail", 1));

                Paragraph referenceNo = section.AddParagraph();
                referenceNo.AddFormattedText(quoteRef, TextFormat.Italic);
                
                Paragraph todaysDate = section.AddParagraph(DateTime.Today.ToString("dd MMMM yyyy"));
                

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

                string address = salutation + Environment.NewLine;
                address = address + pat.ADDRESS1 + Environment.NewLine;
                if (pat.ADDRESS2 != null) //this is sometimes null
                {
                    address = address + pat.ADDRESS2 + Environment.NewLine;
                }
                address = address + pat.ADDRESS3 + Environment.NewLine;
                address = address + pat.ADDRESS4 + Environment.NewLine;
                address = address + pat.POSTCODE;

                //row2.Cells[0].AddParagraph(address);

                spacer = section.AddParagraph();
                spacer = section.AddParagraph();

                Paragraph patAddress = section.AddParagraph(address);
                //patAddress.Format.Font.Size = 12;

                spacer = section.AddParagraph();
                spacer = section.AddParagraph();

                //row2.Cells[1].AddParagraph(contactData);

                Paragraph pSalutation = section.AddParagraph("Dear " + salutation);
                //pSalutation.Format.Font.Size = 12;

                openingBlurb = openingBlurb + " " + clinician.NAME + ", ";
                if (clinician.POSITION.Contains(Environment.NewLine))
                {
                    openingBlurb = openingBlurb + clinician.POSITION.Remove(clinician.POSITION.IndexOf(Environment.NewLine));
                }
                else
                {
                    openingBlurb = openingBlurb + clinician.POSITION;
                }

                openingBlurb = openingBlurb + " at:";

                Paragraph opening = section.AddParagraph(openingBlurb);
                //opening.Format.Font.Size = 12;

                spacer = section.AddParagraph();

                Paragraph venueDetails = section.AddParagraph();
                venueDetails.AddFormattedText(clinic.LOCATION.Replace("  ", Environment.NewLine), TextFormat.Bold);
                //venueDetails.Format.Font.Size = 12;
                venueDetails.Format.Alignment = ParagraphAlignment.Center;

                spacer = section.AddParagraph();
                Paragraph on = section.AddParagraph("on");
                //on.Format.Font.Size = 12;
                on.Format.Alignment = ParagraphAlignment.Center;
                spacer = section.AddParagraph();
                Paragraph dateAndTime = section.AddParagraph();
                dateAndTime.AddFormattedText(appt.BOOKED_DATE.Value.ToString("dddd, dd MMMM yyyy") + " at " + appt.BOOKED_TIME.Value.ToString("HH:mm"), TextFormat.Bold);
                //dateAndTime.Format.Font.Size = 12;
                dateAndTime.Format.Alignment = ParagraphAlignment.Center;
                spacer = section.AddParagraph();
                Paragraph duration = section.AddParagraph("The appointment usually lasts about " + appt.Duration + " minutes.");
                //duration.Format.Font.Size = 12;
                duration.Format.Alignment = ParagraphAlignment.Center;
                spacer = section.AddParagraph();
                if (clinic.NOTES != null && clinic.NOTES != "") //special instructions that may or mat not be present
                {
                    Paragraph notes = section.AddParagraph(clinic.NOTES);
                    //notes.Format.Font.Size = 12;
                }

                spacer = section.AddParagraph();

                Paragraph para3 = section.AddParagraph(); //"What if I can't attend?"
                para3.AddFormattedText(docContent.Para3, TextFormat.Bold);
                //para3.Format.Font.Size = 12;
                para3.Format.Alignment = ParagraphAlignment.Center;

                spacer = section.AddParagraph();
                
                Paragraph para8 = section.AddParagraph(docContent.Para8); //"If you can't attend, please tell someone... etc"
                //para8.Format.Font.Size = 12;

                spacer = section.AddParagraph();

                Paragraph para9 = section.AddParagraph();
                para9.AddFormattedText(docContent.Para9, TextFormat.Bold);
                //para9.Format.Font.Size = 12;
                para9.Format.Alignment = ParagraphAlignment.Center;

                spacer = section.AddParagraph();

                //Paragraph signOff = section.AddParagraph("Yours sincerely," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                //    "Clinical Genetics Booking Centre");
                //signOff.Format.Font.Size = 12;

                MigraDoc.DocumentObjectModel.Tables.Table table2 = section.AddTable(); //tables are used to get stuff to appear side by side
                MigraDoc.DocumentObjectModel.Tables.Column signOff = table2.AddColumn();
                signOff.Format.Alignment = ParagraphAlignment.Left;
                MigraDoc.DocumentObjectModel.Tables.Column qrCode = table2.AddColumn();
                qrCode.Format.Alignment = ParagraphAlignment.Right;

                table2.Rows.Height = 20;
                table2.Columns.Width = 400;
                MigraDoc.DocumentObjectModel.Tables.Row row1_2 = table2.AddRow();
                row1_2.VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Top;

                row1_2.Cells[0].AddParagraph("Yours sincerely," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                    "Clinical Genetics Booking Centre");

                if(clinic.HasQRCode)
                {
                    if (File.Exists($@"wwwroot\Images\QRCodes\QRCode{clinic.FACILITY}.png"))
                    {
                        MigraDoc.DocumentObjectModel.Shapes.Image imgQR = row1_2.Cells[1].AddImage($@"wwwroot\Images\QRCodes\QRCode{clinic.FACILITY}.png");
                        imgQR.ScaleWidth = new Unit(0.18, UnitType.Point);
                        imgQR.ScaleHeight = new Unit(0.18, UnitType.Point);                        
                    }                    
                }


                spacer = section.AddParagraph();
                spacer = section.AddParagraph();

                Paragraph otherStuff = section.AddParagraph();
                otherStuff.AddFormattedText("All personal information contained in your medical notes is held in accordance with the Data Protection Act 1998 and managed " +
                    "securely. We may share this information with service providers outside of this Trust but we would do so only as part of your care. " +
                    "You have a right to object to the disclosure of your personal information. If you do wish to object at any time, or if you would simply like more " +
                    "information, you can contact the department on 0121 335 8024.", TextFormat.Italic);
                otherStuff.Format.Font.Size = 8;

                Border newBorder = new Border { Style = BorderStyle.Single };
                otherStuff.Format.Borders.Top = newBorder;


                Paragraph docCode = section.AddParagraph("AppLetterCTB"); //because of course it's hard-coded, the actual DocCode is "CLINIC" for some reason.
                docCode.Format.Alignment = ParagraphAlignment.Right;
                docCode.Format.Font.Size = 8;


                if (File.Exists($"wwwroot/letter-{username}.pdf"))
                {
                    File.Delete($"wwwroot/letter-{username}.pdf");
                }

                PdfDocumentRenderer pdf = new PdfDocumentRenderer();
                pdf.Document = document;
                pdf.RenderDocument();

                pdf.PdfDocument.Save(Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/letter-{username}.pdf"));


                if (!isEmailOnly)
                {
                    //the synertec printer can't handle PDFs, so we have to use the "copy to folder" method instead
                    string synertecFolderLocation = _constant.GetConstant("SynertecPrintFolder", 1);
                    synertecFolderLocation = synertecFolderLocation.Replace("\\", "/").Trim() + $"/AptLetter-{refID.ToString()}.pdf";

                    if (!File.Exists(synertecFolderLocation))
                    {
                        return 0;
                    }
                    else
                    {
                        File.Copy($"wwwroot/letter-{username}.pdf", synertecFolderLocation);
                    }
                    /*
                    string printerName = _constant.GetConstant("SynertecPrinterName", 1).Trim();
                    
                    using (Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument())
                    {
                        pdf.LoadFromFile($"wwwroot/letter-{username}.pdf");
                        pdf.PrintSettings.PrinterName = printerName;
                        pdf.Print();
                    }*/
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
