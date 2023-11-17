using CPTest.Data;
using CPTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;

namespace CPTest.Pages
{
    public class FinishedModel : PageModel
    {        
        private readonly DataContext _context;

        public FinishedModel(DataContext context)
        {
            _context = context;
        }
        
        public void OnGet()
        {
             
        }     

        
    }
}