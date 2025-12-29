using CPTest.Data;
using CPTest.Models;
using Microsoft.EntityFrameworkCore;


namespace CPTest.Connections
{
    interface INationalHolidayDataAsync
    {
        public Task<List<NationalHolidays>> GetNationalHolidays();
    }
    public class NationalHolidayDataAsync : INationalHolidayDataAsync
    {        
        private readonly CPXContext _context;
        
        public NationalHolidayDataAsync(CPXContext context)
        {            
            _context = context;
        }        
       

        public async Task<List<NationalHolidays>> GetNationalHolidays()
        {            
            IQueryable<NationalHolidays> holidays = _context.NationalHolidays;
        
            return await holidays.ToListAsync();
        }
    }
}
