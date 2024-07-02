using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IExternalClinicianData
    { 
        public ExternalClinician GetClinicianDetails(string cCode);        
    }
    public class ExternalClinicianData : IExternalClinicianData
    {
        private readonly DataContext _context;
        public ExternalClinicianData(DataContext context)
        {
            _context = context;
        }
              

        public ExternalClinician GetClinicianDetails(string cCode)
        {
            ExternalClinician clin = _context.ExternalClinician.FirstOrDefault(s => s.MasterClinicianCode == cCode);
            return clin;
        }        
        
    }
}
