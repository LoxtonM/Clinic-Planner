using CPTest.Connections;
using CPTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CPTest.Pages
{
    public class CGUNumberNotFoundModel : PageModel
    {
        

        public CGUNumberNotFoundModel(){}

        
        public void OnGet(int intID, string clinicianID, string clinicID)
        {
            if (User.Identity.Name is null)
            {
                Response.Redirect("Login");
            }
            
        }
        
    }
}
