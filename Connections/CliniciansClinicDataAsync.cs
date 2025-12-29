using CPTest.Data;
using CPTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Connections
{
    interface ICliniciansClinicDataAsync
    {
        public Task<List<CliniciansClinics>> GetCliniciansClinics(string clinician);
    }
    public class CliniciansClinicDataAsync : ICliniciansClinicDataAsync
    {
        private readonly CPXContext _context;
        public CliniciansClinicDataAsync(CPXContext context)
        {
            _context = context;
        }
        
        
        public async Task<List<CliniciansClinics>> GetCliniciansClinics(string clinician)
        {
            List<CliniciansClinics> clinics = await _context.CliniciansClinics.Where(c => c.STAFF_CODE == clinician).ToListAsync();
            return clinics;
        }
    }
}
