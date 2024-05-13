using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IClinicSlotData
    {
        public IEnumerable<ClinicSlot> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic);
        public IEnumerable<ClinicSlot> GetOpenSlots(IEnumerable<ClinicSlot> clinicSlots);
    }
    public class ClinicSlotData : IClinicSlotData
    {
        private readonly DataContext _context;
        public ClinicSlotData(DataContext context)
        {
            _context = context;
        }
        

        public IEnumerable<ClinicSlot> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
        {
            var slots = _context.ClinicSlots.Where(l => l.SlotDate >= dFrom & l.SlotDate <= dTo).ToList().OrderBy(l => l.SlotDate).ToList();
            
            if (clinician != null)
            {
                slots = slots.Where(s => s.ClinicianID == clinician).ToList();
            }
            if (clinic != null)
            {
                slots = slots.Where(s => s.ClinicID == clinic).ToList();
            }

            return slots;
        }

        public IEnumerable<ClinicSlot> GetOpenSlots(IEnumerable<ClinicSlot> clinicSlots)
        {
            var os  = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
            return os;
        }
    }
}
