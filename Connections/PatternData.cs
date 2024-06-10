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
                DateTime dStartDate, DateTime? dEndDate);
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

        public ClinicPattern GetPatternDetailsByData(string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate)
        {
            var pat = _context.ClinicPattern.FirstOrDefault(p => p.StaffID == clinicianID && p.Clinic == clinicID && p.DyOfWk == dayofWeek &&
                    p.WkOfMth == weekofMonth && p.MthOfYr == sMonthofYear && p.NumSlots == numSlots && p.Duration == duration &&
                    p.StartHr == startHour && p.StartMin == startMin && p.startDate == dStartDate && p.endDate == dEndDate);
            return pat;
        }

        public List<ClinicPattern> GetPatternList(string clinID)
        {
            var patterns = _context.ClinicPattern.Where(p => p.StaffID == clinID).ToList();
            return patterns;
        }
        
    }
}
