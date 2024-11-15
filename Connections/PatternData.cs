using CPTest.Data;
using CPTest.Models;
using System.Data.Entity.Core.Objects;

namespace CPTest.Connections
{
    interface IPatternData
    {
        public ClinicPattern GetPatternDetails(int patID);
        public List<ClinicPattern> GetPatternList(string clinID);

        public ClinicPattern GetPatternDetailsByData(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate);
    }
    public class PatternData : IPatternData
    {
        private readonly CPXContext _context;
        public PatternData(CPXContext context)
        {
            _context = context;
        }
        
        
        public ClinicPattern GetPatternDetails(int patID) 
        {
            ClinicPattern pat = _context.ClinicPattern.FirstOrDefault(p => p.PatternID == patID);
            return pat;
        }

        public ClinicPattern GetPatternDetailsByData(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate)
        {
            ClinicPattern pat = _context.ClinicPattern.FirstOrDefault(p => p.StaffID == clinicianID && p.Clinic == clinicID && p.DyOfWk == dayofWeek &&
                    p.WkOfMth == weekofMonth && p.MthOfYr == sMonthofYear && p.NumSlots == numSlots && p.Duration == duration &&
                    p.StartHr == startHour && p.StartMin == startMin && p.startDate == dStartDate);
            return pat;
        }

        public List<ClinicPattern> GetPatternList(string clinID)
        {
            IQueryable<ClinicPattern> patterns = _context.ClinicPattern.Where(p => p.StaffID == clinID);
            
            return patterns.ToList();
        }
        
    }
}
