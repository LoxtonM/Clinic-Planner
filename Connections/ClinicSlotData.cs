using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IClinicSlotData
    {
        public List<ClinicSlot> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic);
        public List<ClinicSlot> GetOpenSlots(List<ClinicSlot> clinicSlots);
        public List<ClinicSlot> GetMatchingSlots(string clinician, string clinic, DateTime slotdate, int starthr, int startmin, int duration);
        public List<ClinicSlot> GetDaySlots(DateTime slotdate, string? clinician=null, string? clinic=null);
        public ClinicSlot GetSlotDetails(int slotID);
    }
    public class ClinicSlotData : IClinicSlotData
    {
        private readonly DataContext _context;
        public ClinicSlotData(DataContext context)
        {
            _context = context;
        }


        public List<ClinicSlot> GetClinicSlots(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
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

            return slots.ToList();
        }

        public List<ClinicSlot> GetOpenSlots(List<ClinicSlot> clinicSlots)
        {
            IEnumerable<ClinicSlot> os = clinicSlots.Where(l => l.SlotStatus == "Open" || l.SlotStatus == "Unavailable" || l.SlotStatus == "Reserved");
            
            return os.ToList();
        }

        public List<ClinicSlot> GetMatchingSlots(string clinician, string clinic, DateTime slotdate, int starthr, int startmin, int duration)
        {
            IQueryable<ClinicSlot> slots = _context.ClinicSlots.Where(l => l.SlotDate == slotdate && l.StartHr == starthr && l.StartMin == startmin && l.duration == duration
                                                        && l.ClinicianID == clinician && l.ClinicID == clinic);

            return slots.ToList();
        }

        public List<ClinicSlot> GetDaySlots(DateTime slotdate, string? clinician = null, string? clinic = null)
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

            return slots.ToList();
        }


        public ClinicSlot GetSlotDetails(int slotID)
        {
            ClinicSlot slot = _context.ClinicSlots.FirstOrDefault(s => s.SlotID == slotID);

            return slot;
        }
    }
}
