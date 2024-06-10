using CPTest.Data;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace CPTest.Connections
{
    interface IClinicSlotsCreator
    {      
        public void SetupClinicPattern(int patternID, string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate, string username);
        public void SetupAdHocClinic(int adHocID, DateTime clinicDate, int startHr, int startMin, string clinicianID,
            string clinicID, int duration, int numSlots, string username);     
    }
    public class ClinicSlotsCreator : IClinicSlotsCreator
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IClinicSlotData _slotData;
        private readonly IMiscData _dc;
        private readonly IClinicSlotSqlServices _ssSlot;

        public ClinicSlotsCreator(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _slotData = new ClinicSlotData(_context);
            _dc = new MiscData(_context);
            _ssSlot = new ClinicSlotSqlServices(_config);
        }
                
        public void SetupClinicPattern(int patternID, string clinicianID, string clinicID, int dayofWeek, int weekofMonth,
                string sMonthofYear, int numSlots, int duration, int startHour, int startMin,
                DateTime dStartDate, DateTime? dEndDate, string username)
        {
            //var pattern = _patternData.GetPatternDetails(patternID);          

            List <DateTime> ClinicDates = new List<DateTime>(); //list of all clinic dates, to be populated later

            DateTime firstDayCurrentMonth = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + 1);
            DateTime firstDateCurrentMonth;
            DateTime firstOccurrenceCurrentMonth;
            DateTime firstDayNextMonth;
            DateTime firstDateNextMonth;
            DateTime firstOccurrenceNextMonth;
            DateTime nextClinicDate;
            DateTime slotTime = DateTime.Parse("1899-12-30 " + startHour.ToString() + ":" + startMin.ToString() + ":00");
            DateTime startDate;
            DateTime endDate;
            int iMonth;
            int iYear;

            startDate = dStartDate;
            endDate = dEndDate.GetValueOrDefault();            

            if(endDate == DateTime.Parse("0001-01-01")) //set a default end date if none specified
            {
                endDate = DateTime.Now.AddYears(1);
            }

            if (GetIsMonthInPattern(sMonthofYear, firstDayCurrentMonth)) //check if month is in the pattern
            {
                firstOccurrenceCurrentMonth = FindFirstOccurrence(firstDayCurrentMonth, dayofWeek);
                firstDateCurrentMonth = firstOccurrenceCurrentMonth.AddDays(-7 + (weekofMonth * 7));
            }
            else
            {
                firstDateCurrentMonth = startDate.AddDays(-1);
            }

            iMonth = firstDateCurrentMonth.Month + 1;
            iYear = firstDateCurrentMonth.Year;

            if (iMonth == 13) 
            {
                iMonth = 1;
                iYear += 1;
            }

            firstDayNextMonth = DateTime.Parse(iYear + "-" + iMonth + "-" + 1);
            if (GetIsMonthInPattern(sMonthofYear, firstDayNextMonth))
            {
                firstOccurrenceNextMonth = FindFirstOccurrence(firstDayNextMonth, dayofWeek);
                firstDateNextMonth = firstOccurrenceNextMonth.AddDays(-7 + (weekofMonth * 7));
            }
            else
            {
                firstDateNextMonth = DateTime.Now.AddDays(-1);
            }

            if(firstDateCurrentMonth > DateTime.Now)
            {
                nextClinicDate = firstDateCurrentMonth;
            }
            else
            {
                nextClinicDate = firstDateNextMonth;
            }

            if(GetIsMonthInPattern(sMonthofYear, nextClinicDate) && nextClinicDate >= startDate && !_dc.GetIsNationalHoliday(nextClinicDate))
            {
                ClinicDates.Add(nextClinicDate);
            }

            do //goes through the months and creates a list of clinic dates
            {
                iMonth = nextClinicDate.Month + 1;
                if (iMonth == 13)
                {
                    iMonth = 1;
                    iYear += 1;
                }
                firstDayNextMonth = DateTime.Parse(iYear + "-" + iMonth + "-" + 1);
                if (GetIsMonthInPattern(sMonthofYear, firstDayNextMonth))
                {
                    firstOccurrenceNextMonth = FindFirstOccurrence(firstDayNextMonth, dayofWeek);
                    int sdf = -7 + (weekofMonth* 7);
                    DateTime asdfgf = firstOccurrenceNextMonth.AddDays(-7 + (weekofMonth * 7));
                    firstDateNextMonth = firstOccurrenceNextMonth.AddDays(-7 + (weekofMonth * 7));
                    nextClinicDate = firstDateNextMonth;
                    ClinicDates.Add(nextClinicDate);
                }
                else
                {
                    nextClinicDate = firstDayNextMonth;
                }                
            }
            while (nextClinicDate <= endDate);

            

            foreach (var date in ClinicDates) 
            {
                CreateDayOfClinicSlots(clinicianID, clinicID, date, slotTime, numSlots, duration, username, patternID);
            }
        }

        public void SetupAdHocClinic(int adHocID, DateTime clinicDate, int startHr, int startMin, string clinicianID,
        string clinicID, int duration, int numSlots, string username)
        {
            DateTime dStartTime = DateTime.Parse("1899-12-30 " + startHr.ToString() + ":" + startMin.ToString() + ":00");            

            CreateDayOfClinicSlots(clinicianID, clinicID, clinicDate, dStartTime, numSlots, duration, username, adHocID);
        }

        private void CreateDayOfClinicSlots(string clinician, string clinic, DateTime slotDate,
            DateTime StartTime, int numSlots, int duration, string sUsername, int iPatternID)
        {
            DateTime slotTime;

            for (int i = 0; i < numSlots; i++) //adds the time slots from the duration
            {
                slotTime = StartTime.AddMinutes(i * duration);

                int starthr = slotTime.Hour;
                int startmin = slotTime.Minute;

                if (!IsExistingClinicSlot(slotDate, starthr, startmin, clinician, clinic, duration))
                {
                    _ssSlot.CreateClinicSlot(slotDate, slotTime, clinician, clinic, sUsername, duration, iPatternID);
                }
            }
        }
        private bool GetIsMonthInPattern(string months, DateTime dDate)
        {
            string monthToCompare = dDate.Month.ToString();
            if (monthToCompare == "10") monthToCompare = "a";
            if (monthToCompare == "11") monthToCompare = "b";
            if (monthToCompare == "12") monthToCompare = "c";

            if(months.Contains(monthToCompare)) return true;

            return false;
        }

        private DateTime FindFirstOccurrence(DateTime date, int intDay)
        {
            DateTime FirstDate = new DateTime();
            string strDay = "";

            if (intDay == 1) strDay = "Monday";
            if (intDay == 2) strDay = "Tuesday";
            if (intDay == 3) strDay = "Wednesday";
            if (intDay == 4) strDay = "Thursday";
            if (intDay == 5) strDay = "Frday";

            FirstDate = _dc.GetFirstDateFromList(date, strDay);
            return FirstDate;
        }

        private bool IsExistingClinicSlot(DateTime slotDate, int starthr, int startmin, string clinician, string clinic, int duration) 
        {
            if (_slotData.GetMatchingSlots(clinician, clinic, slotDate, starthr, startmin, duration).Count() > 0 ) return true;

            return false;
        }
    }
}
