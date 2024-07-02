using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IOutcomeData
    {
        public List<Outcome> GetOutcomeList();
    }
    public class OutcomeData : IOutcomeData
    {
        private readonly DataContext _context;
        public OutcomeData(DataContext context)
        {
            _context = context;
        }
       
        public List<Outcome> GetOutcomeList() 
        {
            IQueryable<Outcome> oc = _context.Outcomes.Where(o => o.DEFAULT_CLINIC_STATUS == "Active");

            return oc.ToList();
        }
    }
}
