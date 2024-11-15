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
        private readonly CPXContext _context;
        public ClinicDetailsData(CPXContext context)
        {
            _context = context;
        }        

        public ClinicDetails GetClinicDetails(string ven)
        {
            ClinicDetails clin = _context.ClinicDetails.FirstOrDefault(v => v.Facility == ven);
            return clin;
        }

        
    }
}
