using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IClinicVenueData
    {
        public ClinicVenue GetVenueDetails(string ven);
        public List<ClinicVenue> GetVenueList();
    }
    public class ClinicVenueData : IClinicVenueData
    {
        private readonly DataContext _context;
        public ClinicVenueData(DataContext context)
        {
            _context = context;
        }        

        public ClinicVenue GetVenueDetails(string ven)
        {
            ClinicVenue clin = _context.ClinicVenues.FirstOrDefault(v => v.FACILITY == ven);
            return clin;
        }

        public List<ClinicVenue> GetVenueList() 
        {
            IQueryable<ClinicVenue> venuelist = _context.ClinicVenues.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
            return venuelist.ToList();
        }       
    }
}
