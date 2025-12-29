using Microsoft.AspNetCore.Mvc.RazorPages;


namespace CPTest.Pages
{
    public class FinishedModel : PageModel
    {   
        public void OnGet()
        {
            if (User.Identity.Name is null)
            {
                Response.Redirect("Login");
            }
        }     

        
    }
}