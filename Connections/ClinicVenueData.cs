using CPTest.Data;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;

namespace CPTest.Connections
{
    interface IClinicVenueData
    {
        public ClinicVenue GetVenueDetails(string ven);
        public List<ClinicVenue> GetVenueList();
    }
    public class ClinicVenueData : IClinicVenueData
    {
        private readonly ClinicalContext _context;
        private readonly CPXContext _cpxContext;
        public ClinicVenueData(ClinicalContext context, CPXContext cpxContext)
        {
            _context = context;
            _cpxContext = cpxContext;
        }        

        public ClinicVenue GetVenueDetails(string ven)
        {
            ClinicVenue clin = _context.ClinicalFacilities.FirstOrDefault(v => v.FACILITY == ven);
            return clin;
        }

        public List<ClinicVenue> GetVenueList() 
        {
            IQueryable<ClinicVenue> venuelist = _context.ClinicalFacilities.Where(v => v.NON_ACTIVE == 0).OrderBy(v => v.NAME);
            return venuelist.ToList();
        }       
    }
}
