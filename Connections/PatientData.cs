using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IPatientData
    {
        public Patient GetPatientDetails(int mpi);
        public Patient GetPatientDetailsByIntID(int intID);
        public Patient GetPatientDetailsByCGUNo(string cguNo);
        public List<Patient> GetFamilyMembers(int mpi);
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
            Patient pt = _context.Patients.FirstOrDefault(p => p.MPI == mpi);            
            return pt;
        }
        public Patient GetPatientDetailsByIntID(int intID)
        {
            Patient pt = _context.Patients.FirstOrDefault(p => p.INTID == intID);           
            return pt;
        }

        public Patient GetPatientDetailsByCGUNo(string cguNo)
        {
            Patient pt = _context.Patients.FirstOrDefault(p => p.CGU_No == cguNo);
            return pt;
        }

        public List<Patient> GetFamilyMembers(int mpi)
        {
            Patient patient = _context.Patients.FirstOrDefault(p => p.MPI == mpi);
            IQueryable<Patient> pts = _context.Patients.Where(p => p.PEDNO == patient.PEDNO & p.MPI != patient.MPI).OrderBy(p => p.MPI);
            return pts.ToList();
        }

    }
}
