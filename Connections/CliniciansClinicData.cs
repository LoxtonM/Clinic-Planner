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
        private readonly CPXContext _context;
        public CliniciansClinicData(CPXContext context)
        {
            _context = context;
        }
        
        
        public List<CliniciansClinics> GetCliniciansClinics(string clinician)
        {
            List<CliniciansClinics> clinics = _context.CliniciansClinics.Where(c => c.STAFF_CODE == clinician).ToList();
            return clinics;
        }
    }
}
