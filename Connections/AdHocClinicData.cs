using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IAdHocClinicData
    {
        public ClinicsAdded GetAdHocClinicDetails(int id);
        public List<ClinicsAdded> GetAdHocList(string clinID);
    }
    public class AdHocClinicData : IAdHocClinicData
    {
        private readonly DataContext _context;
        public AdHocClinicData(DataContext context)
        {
            _context = context;
        }
                
        public ClinicsAdded GetAdHocClinicDetails(int id)
        {
            var adhoc = _context.ClinicsAdded.FirstOrDefault(p => p.ID == id);
            return adhoc;
        }
        public List<ClinicsAdded> GetAdHocList(string clinID) 
        {
            var adhocs = _context.ClinicsAdded.Where(c => c.ClinicianID == clinID).ToList();
            return adhocs;
        }
                
    }
}
