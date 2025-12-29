using CPTest.Data;
using CPTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CPTest.Connections
{
    interface ICancellationReasonDataAsync
    {
        public Task<List<CancellationReason>> GetCancellationReasonsList();
    }
    public class CancellationReasonDataAsync : ICancellationReasonDataAsync
    {
        private readonly CPXContext _context;
        public CancellationReasonDataAsync(CPXContext context)
        {
            _context = context;
        }
       
        public async Task<List<CancellationReason>> GetCancellationReasonsList() 
        {
            IQueryable<CancellationReason> cr = _context.CancellationReasons;

            return await cr.ToListAsync();
        }
    }
}
