using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IWaitingListData
    {
        public IEnumerable<WaitingList> GetWaitingList(string? clinician, string? clinic);
        public IEnumerable<WaitingList> GetWaitingListByCGUNo(string searchTerm);
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
            var wl = _context.WaitingList.ToList();

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
            var wl = _context.WaitingList.Where(w => w.CGU_No.Contains(searchTerm));                       
            return wl.OrderBy(l => l.AddedDate);
        }        
    }
}
