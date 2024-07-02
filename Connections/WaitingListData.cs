using CPTest.Data;
using CPTest.Models;
using System.Data.Entity.Core.Mapping;

namespace CPTest.Connections
{
    interface IWaitingListData
    {
        public IEnumerable<WaitingList> GetWaitingList(string? clinician, string? clinic);
        public IEnumerable<WaitingList> GetWaitingListByCGUNo(string searchTerm);
        public WaitingList GetWaitingListEntry(int intID, string clinicianID, string clinicID);
    }
    public class WaitingListData : IWaitingListData
    {
        private readonly DataContext _context;
        public WaitingListData(DataContext context)
        {
            _context = context;
        }
       

        public IEnumerable<WaitingList> GetWaitingList(string? clinician, string? clinic)
        {
            List<WaitingList> wl = _context.WaitingList.ToList();

            if (clinician != null)
            {
                wl = wl.Where(l => l.ClinicianID == clinician).ToList();
            }
            if (clinic != null)
            {
                wl = wl.Where(l => l.ClinicID == clinic).ToList();
            }
            return wl.OrderBy(l => l.PriorityLevel).ThenBy(l => l.AddedDate);
        }

        public IEnumerable<WaitingList> GetWaitingListByCGUNo(string searchTerm)
        {
            IQueryable<WaitingList> wl = _context.WaitingList.Where(w => w.CGU_No.Contains(searchTerm));                       
            return wl.OrderBy(l => l.AddedDate);
        }
        
        public WaitingList GetWaitingListEntry(int intID, string clinicianID, string clinicID)
        {
            WaitingList waitingList = _context.WaitingList.FirstOrDefault(w => w.IntID == intID && w.ClinicID == clinicID && w.ClinicianID == clinicianID);

            return waitingList;
        }
    }
}
