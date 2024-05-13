using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface ICliniciansClinicData
    {
        public List<CliniciansClinics> GetCliniciansClinics(string clinician);
    }
    public class CliniciansClinicData : ICliniciansClinicData
    {
        private readonly DataContext _context;
        public CliniciansClinicData(DataContext context)
        {
            _context = context;
        }
        
        
        public List<CliniciansClinics> GetCliniciansClinics(string clinician)
        {
            var clinics = _context.CliniciansClinics.Where(c => c.STAFF_CODE == clinician).ToList();
            return clinics;
        }
    }
}
