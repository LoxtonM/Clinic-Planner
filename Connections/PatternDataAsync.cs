using CPTest.Data;
using CPTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Connections
{
    interface IPatternDataAsync
    {
        public Task <ClinicPattern> GetPatternDetails(int patID);
        public Task<List<ClinicPattern>> GetPatternList(string clinID);
        public Task<ClinicPattern> GetPatternDetailsByData(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate);
    }
    public class PatternDataAsync : IPatternDataAsync
    {
        private readonly CPXContext _context;
        public PatternDataAsync(CPXContext context)
        {
            _context = context;
        }
        
        
        public async Task<ClinicPattern> GetPatternDetails(int patID) 
        {
            ClinicPattern pat = await _context.ClinicPattern.FirstAsync(p => p.PatternID == patID);
            return pat;
        }

        public async Task<ClinicPattern> GetPatternDetailsByData(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate)
        {
            ClinicPattern pat = await _context.ClinicPattern.FirstAsync(p => p.StaffID == clinicianID && p.Clinic == clinicID && p.DyOfWk == dayofWeek &&
                    p.WkOfMth == weekofMonth && p.MthOfYr == sMonthofYear && p.NumSlots == numSlots && p.Duration == duration &&
                    p.StartHr == startHour && p.StartMin == startMin && p.startDate == dStartDate);
            return pat;
        }

        public async Task<List<ClinicPattern>> GetPatternList(string clinID)
        {
            IQueryable<ClinicPattern> patterns = _context.ClinicPattern.Where(p => p.StaffID == clinID);
            
            return await patterns.ToListAsync();
        }
        
    }
}
