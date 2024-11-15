using CPTest.Data;
using CPTest.Models;
using System;

namespace CPTest.Connections
{
    interface IAdHocClinicData
    {
        public ClinicsAdded GetAdHocClinicDetails(int id);
        public ClinicsAdded GetAdHocClinicDetailsByData(string clinicianID, string clinicID, int numSlots, int duration, int startHour, int startMin,
                DateTime clinicDate);
        public List<ClinicsAdded> GetAdHocList(string clinID);
    }
    public class AdHocClinicData : IAdHocClinicData
    {
        private readonly CPXContext _context;
        public AdHocClinicData(CPXContext context)
        {
            _context = context;
        }
                
        public ClinicsAdded GetAdHocClinicDetails(int id)
        {
            ClinicsAdded adhoc = _context.ClinicsAdded.FirstOrDefault(p => p.ID == id);
            return adhoc;
        }

        public ClinicsAdded GetAdHocClinicDetailsByData(string clinicianID, string clinicID, int numSlots, int duration, int startHour, int startMin,
                DateTime clinicDate)
        {
            ClinicsAdded adhoc = _context.ClinicsAdded.FirstOrDefault(p => p.ClinicianID == clinicianID && p.ClinicID== clinicID && p.NumSlots == numSlots && 
            p.Duration == duration && p.StartHr == startHour && p.StartMin == startMin && p.ClinicDate == clinicDate);
            return adhoc;
        }
        public List<ClinicsAdded> GetAdHocList(string clinID) 
        {
            List<ClinicsAdded> adhocs = _context.ClinicsAdded.Where(c => c.ClinicianID == clinID).ToList();
            return adhocs;
        }
                
    }
}
