using CPTest.Data;
using CPTest.Models;
using System.Data.Entity;

namespace CPTest.Connections
{
    interface IClinicDetailsDataAsync
    {
        public Task<ClinicDetails> GetClinicDetails(string ven);       
    }
    public class ClinicDetailsDataAsync : IClinicDetailsDataAsync
    {
        private readonly CPXContext _context;
        public ClinicDetailsDataAsync(CPXContext context)
        {
            _context = context;
        }        

        public async Task<ClinicDetails> GetClinicDetails(string ven)
        {
            ClinicDetails clin = await _context.ClinicDetails.FirstAsync(v => v.Facility == ven);
            return clin;
        }

        
    }
}
