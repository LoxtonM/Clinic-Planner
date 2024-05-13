using CPTest.Data;

namespace CPTest.Connections
{
    public class MiscData
    {
        private readonly DataContext _context;
        public MiscData(DataContext context)
        {
            _context = context;
        }
        

        public DateTime GetFirstDateFromList(DateTime dateToCheck, string day) 
        {
            DateTime firstDate = _context.DateList.FirstOrDefault(d => d.Dt.Month == dateToCheck.Month && d.Dt.Year == dateToCheck.Year
                                            && d.NumberOfThisWeekDayInMonth == 1 && d.WeekDay == day).Dt;

            return firstDate;
        }

        public bool GetIsNationalHolday(DateTime date)
        {
            int holdays = _context.NationalHoldays.Where(d => d.HoldayDate == date).Count();

            if(holdays > 0)
            {
                return true;
            }
            return false;
        }
    }
}
