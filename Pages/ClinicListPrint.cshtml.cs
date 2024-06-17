using CPTest.Connections;
using CPTest.Data;
using CPTest.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class ClinicListPrintModel : PageModel
    {
        private readonly DocumentController _doc;
        private readonly DataContext _context;
        private readonly IConfiguration _config;     

        public ClinicListPrintModel(DataContext context, IConfiguration config)
        {
            _context = context;
            _doc = new DocumentController(_context);
            _config = config;  
        }
        public void OnGet(int refID)
        {
            try
            {
                if (_doc.ClinicList(refID) == 1)
                {                   

                    Response.Redirect(@Url.Content(@"~/cliniclist.pdf"));
                }
                else
                {
                    string message = "Something went wrong and the letter didn't print for some reason.";
                    Response.Redirect("Error?sError=" + message);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
    }
}
