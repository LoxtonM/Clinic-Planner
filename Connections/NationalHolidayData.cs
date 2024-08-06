using CPTest.Data;
using CPTest.Models;


namespace CPTest.Connections
{
    interface INationalHolidayData
    {
        public List<NationalHolidays> GetNationalHolidays();
    }
    public class NationalHolidayData : INationalHolidayData
    {        
        private readonly DataContext _context;
        
        public NationalHolidayData(DataContext context)
        {            
            _context = context;
        }        
       

        public List<NationalHolidays> GetNationalHolidays()
        {            
            IQueryable<NationalHolidays> holidays = _context.NationalHolidays;
        
            return holidays.ToList();
        }
    }
}
