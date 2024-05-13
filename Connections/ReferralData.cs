using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IReferralData
    {
        public List<Referral> GetReferralsList(int mpi);
    }
    public class ReferralData : IReferralData
    {
        private readonly DataContext _context;
        public ReferralData(DataContext context)
        {
            _context = context;
        }
       
        public List<Referral> GetReferralsList(int mpi) 
        {
            var refs = _context.Referrals.Where(r => r.MPI == mpi & r.logicaldelete == false & r.COMPLETE == "Active").OrderBy(r => r.RefDate).ToList();
            return refs;
        }
    }
}
