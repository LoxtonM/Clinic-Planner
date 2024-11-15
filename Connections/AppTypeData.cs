using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IAppTypeData
    {
        public List<AppType> GetAppTypeList();
    }
    public class AppTypeData : IAppTypeData
    {
        private readonly CPXContext _context;
        public AppTypeData(CPXContext context)
        {
            _context = context;
        }
       
        public List<AppType> GetAppTypeList()
        {
            List<AppType> at = _context.AppType.Where(t => t.NON_ACTIVE == 0 & t.ISAPPT == true).ToList();
            return at;
        }
    }
}
