using CPTest.Data;
using CPTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Connections
{
    interface IApptsAndSlotsDataAsync
    {        
        public Task<List<ApptsAndSlots>> GetApptsAndSlotsList();
    }
    public class ApptsAndSlotsDataAsync : IApptsAndSlotsDataAsync
    {
        private readonly CPXContext _context;
        public ApptsAndSlotsDataAsync(CPXContext context)
        {
            _context = context;
        }

        public async Task<List<ApptsAndSlots>> GetApptsAndSlotsList()
        {
            IQueryable<ApptsAndSlots> a = _context.ApptsAndSlots;
            
            return await a.ToListAsync();
        }

    }
}
