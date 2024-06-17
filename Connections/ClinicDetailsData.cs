using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IClinicDetailsData
    {
        public ClinicDetails GetClinicDetails(string ven);       
    }
    public class ClinicDetailsData : IClinicDetailsData
    {
        private readonly DataContext _context;
        public ClinicDetailsData(DataContext context)
        {
            _context = context;
        }        

        public ClinicDetails GetClinicDetails(string ven)
        {
            var clin = _context.ClinicDetails.FirstOrDefault(v => v.Facility == ven);
            return clin;
        }

        
    }
}
