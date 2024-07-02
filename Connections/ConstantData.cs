using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IConstantData
    {
        public string GetConstantValue(string constantCode);
    }
    public class ConstantData : IConstantData
    {
        private readonly DataContext _context;
        public ConstantData(DataContext context)
        {
            _context = context;
        }
        
        public string GetConstantValue(string constantCode)
        {
            Constants cnst = _context.Constants.FirstOrDefault(c => c.ConstantCode.Contains(constantCode));
            //We must use "contains" because half of the constant codes have trailing spaces for some reason!
            return cnst.ConstantValue;
        }
        
    }
}
