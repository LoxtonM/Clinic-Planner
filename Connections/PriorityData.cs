using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IPriorityData
    {
        public List<Priority> GetPriorityList();
    }
    public class PriorityData : IPriorityData
    {
        private readonly DataContext _context;
        public PriorityData(DataContext context)
        {
            _context = context;
        }
        
        public List<Priority> GetPriorityList() 
        {            
            var prList = _context.Priority.Where(p => p.IsActive == true).OrderBy(p => p.PriorityLevel);
            return prList.ToList();
        }        
    }
}
