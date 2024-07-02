using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IReferralData
    {
        public List<Referral> GetReferralsList(int mpi);
        public Referral GetReferralDetails(int refID);
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
            IQueryable<Referral> refs = _context.Referrals.Where(r => r.MPI == mpi & r.logicaldelete == false & r.COMPLETE == "Active").OrderBy(r => r.RefDate);
            return refs.ToList();
        }

        public Referral GetReferralDetails(int refID)
        {
            Referral referral = _context.Referrals.FirstOrDefault(r => r.RefID == refID);
            return referral;
        }
    }
}
