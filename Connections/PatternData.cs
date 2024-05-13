using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IPatternData
    {
        public ClinicPattern GetPatternDetails(int patID);
        public List<ClinicPattern> GetPatternList(string clinID);
    }
    public class PatternData : IPatternData
    {
        private readonly DataContext _context;
        public PatternData(DataContext context)
        {
            _context = context;
        }
        
        
        public ClinicPattern GetPatternDetails(int patID) 
        {
            var pat = _context.ClinicPattern.FirstOrDefault(p => p.PatternID == patID);
            return pat;
        }

        public List<ClinicPattern> GetPatternList(string clinID)
        {
            var patterns = _context.ClinicPattern.Where(p => p.StaffID == clinID).ToList();
            return patterns;
        }
        
    }
}
