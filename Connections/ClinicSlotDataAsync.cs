using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Connections
{
    interface IClinicSlotDataAsync
    {
        public Task<List<ClinicSlot>> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic);
        public List<ClinicSlot> GetOpenSlots(List<ClinicSlot> clinicSlots);
        public Task<List<ClinicSlot>> GetMatchingSlots(string clinician, string clinic, DateTime slotdate, int starthr, int startmin, int duration);
        public Task<List<ClinicSlot>> GetDaySlots(DateTime slotdate, string? clinician=null, string? clinic=null);
        public Task<ClinicSlot> GetSlotDetails(int slotID);
    }
    public class ClinicSlotDataAsync : IClinicSlotDataAsync
    {
        private readonly ClinicalContext _context;
        public ClinicSlotDataAsync(ClinicalContext context)
        {
            _context = context;
        }


        public async Task<List<ClinicSlot>> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
        {
            IQueryable<ClinicSlot> slots = _context.ClinicSlots.Where(l => l.SlotDate >= dFrom & l.SlotDate <= dTo).OrderBy(l => l.SlotDate);

            if (clinician != null)
            {
                slots = slots.Where(s => s.ClinicianID == clinician);
            }
            if (clinic != null)
            {
                slots = slots.Where(s => s.ClinicID == clinic);
            }

            return await slots.ToListAsync();
        }

        public List<ClinicSlot> GetOpenSlots(List<ClinicSlot> clinicSlots) //this can not be asynchronous, it simply won't have it.
        {
            IEnumerable<ClinicSlot> os = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
            
            return clinicSlots.ToList();
        }

        public async Task<List<ClinicSlot>> GetMatchingSlots(string clinician, string clinic, DateTime slotdate, int starthr, int startmin, int duration)
        {
            IQueryable<ClinicSlot> slots = _context.ClinicSlots.Where(l => l.SlotDate == slotdate && l.StartHr == starthr && l.StartMin == startmin && l.duration == duration
                                                        && l.ClinicianID == clinician && l.ClinicID == clinic);

            return await slots.ToListAsync();
        }

        public async Task<List<ClinicSlot>> GetDaySlots(DateTime slotdate, string? clinician = null, string? clinic = null)
        {
            IQueryable<ClinicSlot> slots = _context.ClinicSlots.Where(l => l.SlotDate == slotdate);

            if(clinician != null)
            {
                slots = slots.Where(l => l.ClinicianID == clinician);
            }

            if(clinic != null)
            {
                slots = slots.Where(l => l.ClinicID == clinic);
            }

            return await slots.ToListAsync();
        }


        public async Task<ClinicSlot> GetSlotDetails(int slotID)
        {
            ClinicSlot slot = await _context.ClinicSlots.FirstAsync(s => s.SlotID == slotID);

            return slot;
        }
    }
}
