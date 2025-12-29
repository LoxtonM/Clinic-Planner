using CPTest.Data;
using CPTest.Models;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Connections
{
    interface IAdHocClinicDataAsync
    {
        public Task<ClinicsAdded> GetAdHocClinicDetails(int id);
        public Task<ClinicsAdded> GetAdHocClinicDetailsByData(string clinicianID, string clinicID, int numSlots, int duration, int startHour, int startMin,
                DateTime clinicDate);
        public Task<List<ClinicsAdded>> GetAdHocList(string clinID);
    }
    public class AdHocClinicDataAsync : IAdHocClinicDataAsync
    {
        private readonly CPXContext _context;
        public AdHocClinicDataAsync(CPXContext context)
        {
            _context = context;
        }
                
        public async Task<ClinicsAdded> GetAdHocClinicDetails(int id)
        {
            ClinicsAdded adhoc = await _context.ClinicsAdded.FirstAsync(p => p.ID == id);
            return adhoc;
        }

        public async Task<ClinicsAdded> GetAdHocClinicDetailsByData(string clinicianID, string clinicID, int numSlots, int duration, int startHour, int startMin,
                DateTime clinicDate)
        {
            ClinicsAdded adhoc = await _context.ClinicsAdded.FirstAsync(p => p.ClinicianID == clinicianID && p.ClinicID== clinicID && p.NumSlots == numSlots && 
            p.Duration == duration && p.StartHr == startHour && p.StartMin == startMin && p.ClinicDate == clinicDate);
            return adhoc;
        }
        public async Task<List<ClinicsAdded>> GetAdHocList(string clinID) 
        {
            List<ClinicsAdded> adhocs = await _context.ClinicsAdded.Where(c => c.ClinicianID == clinID).ToListAsync();
            return adhocs;
        }
                
    }
}
