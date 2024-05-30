using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IPatientData
    {
        public Patient GetPatientDetails(int mpi);
        public Patient GetPatientDetailsByIntID(int intID);

        public Patient GetPatientDetailsByCGUNo(string cguNo);
    }
    public class PatientData : IPatientData
    { 
        private readonly DataContext _context;
        public PatientData(DataContext context)
        {
            _context = context;
        }

        public Patient GetPatientDetails(int mpi)
        {
            var pt = _context.Patients.FirstOrDefault(p => p.MPI == mpi);            
            return pt;
        }
        public Patient GetPatientDetailsByIntID(int intID)
        {
            var pt = _context.Patients.FirstOrDefault(p => p.INTID == intID);           
            return pt;
        }

        public Patient GetPatientDetailsByCGUNo(string cguNo)
        {
            var pt = _context.Patients.FirstOrDefault(p => p.CGU_No == cguNo);
            return pt;
        }

    }
}
