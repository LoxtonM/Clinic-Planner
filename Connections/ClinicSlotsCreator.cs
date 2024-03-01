using CPTest.Data;

namespace CPTest.Connections
{
    public class ClinicSlotsCreator
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private DataConnections dc;
        private SqlServices sql;

        public ClinicSlotsCreator(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            dc = new DataConnections(_context);
            sql = new SqlServices(_config);
        }
        public void SetupClinicPattern(string sClinician, string sClinic, DateTime dStart, DateTime? dEnd,
            int startHr, int startMin, int numSlots, int duration, int DayOfWeek, int WeekOfMonth, 
            string MonthOfYear, string sUsername) //creates slots for a standard clinic pattern
        {
            sql.SaveClinicPattern(sClinician, sClinic, DayOfWeek, WeekOfMonth, MonthOfYear, numSlots, duration,
                startHr, startMin, dStart, dEnd);

            int iPatternID = _context.ClinicPattern.FirstOrDefault(p => p.StaffID == sClinician && p.Clinic == sClinic && 
                                p.DyOfWk == DayOfWeek && p.WkOfMth == WeekOfMonth && p.startDate == dStart && p.MthOfYr == MonthOfYear
                                && p.StartHr == startHr && p.StartMin == startMin).PatternID;

            List<DateTime> ClinicDates = new List<DateTime>(); //list of all clinic dates, to be populated later

            DateTime firstDayCurrentMonth = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + 1);
            DateTime firstDateCurrentMonth;
            DateTime firstOccurrenceCurrentMonth;
            DateTime firstDayNextMonth;
            DateTime firstDateNextMonth;
            DateTime firstOccurrenceNextMonth;
            DateTime nextClinicDate;
            DateTime slotTime = DateTime.Parse("1899-12-30 " + startHr.ToString() + ":" + startMin.ToString() + ":00");
            int iMonth;
            int iYear;

            if(dEnd == DateTime.Parse("0001-01-01")) //set a default end date if none specified
            {
                dEnd = DateTime.Now.AddYears(1);
            }

            if (GetIsMonthInPattern(MonthOfYear, firstDayCurrentMonth)) //check if month is in the pattern
            {
                firstOccurrenceCurrentMonth = FindFirstOccurrence(firstDayCurrentMonth, DayOfWeek);
                firstDateCurrentMonth = firstOccurrenceCurrentMonth.AddDays(-7 + (WeekOfMonth * 7));
            }
            else
            {
                firstDateCurrentMonth = dStart.AddDays(-1);
            }

            iMonth = firstDateCurrentMonth.Month + 1;
            iYear = firstDateCurrentMonth.Year;

            if (iMonth == 13) 
            {
                iMonth = 1;
                iYear += 1;
            }

            firstDayNextMonth = DateTime.Parse(iYear + "-" + iMonth + "-" + 1);
            if (GetIsMonthInPattern(MonthOfYear, firstDayNextMonth))
            {
                firstOccurrenceNextMonth = FindFirstOccurrence(firstDayNextMonth, DayOfWeek);
                firstDateNextMonth = firstOccurrenceNextMonth.AddDays(-7 + (WeekOfMonth * 7));
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

            if(GetIsMonthInPattern(MonthOfYear, nextClinicDate) && nextClinicDate >= dStart && !dc.GetIsNationalHoliday(nextClinicDate))
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
                if (GetIsMonthInPattern(MonthOfYear, firstDayNextMonth))
                {
                    firstOccurrenceNextMonth = FindFirstOccurrence(firstDayNextMonth, DayOfWeek);
                    firstDateNextMonth = firstOccurrenceNextMonth.AddDays(7 - (WeekOfMonth * 7));                //DateAdd("ww", intWeek - 1, firstOccurrenceNextMonth)
                    nextClinicDate = firstDateNextMonth;
                    ClinicDates.Add(nextClinicDate);
                }
                else
                {
                    nextClinicDate = firstDayNextMonth;
                }                
            }
            while (nextClinicDate <= dEnd);

            

            foreach (var date in ClinicDates) 
            {
                CreateDayOfClinicSlots(sClinician, sClinic, date, slotTime, numSlots, duration, sUsername, iPatternID);
            }
        }

        public void SetupAdHocClinic(DateTime dClinicDate, int iStartHr, int iStartMin, string sClinicianID,
        string sClinicID, int iDuration, int iNumSlots, string sStaffCode)
        {
            DateTime dStartTime = DateTime.Parse("1899-12-30 " + iStartHr.ToString() + ":" + iStartMin.ToString() + ":00");

            sql.SaveAdHocClinic(sClinicianID, sClinicID, iNumSlots, iDuration, iStartHr, iStartMin, dClinicDate);

            int iPatternID = _context.ClinicsAdded.FirstOrDefault(p => p.ClinicianID == sClinicianID && p.ClinicID == sClinicID &&
                                p.ClinicDate == dClinicDate && p.StartHr == iStartHr && p.StartMin == iStartMin).ID;

            CreateDayOfClinicSlots(sClinicianID, sClinicID, dClinicDate, dStartTime, iNumSlots, iDuration, sStaffCode, iPatternID);
        }

        private void CreateDayOfClinicSlots(string sClinician, string sClinic, DateTime slotDate,
            DateTime StartTime, int numSlots, int duration, string sUsername, int iPatternID)
        {
            DateTime slotTime;

            for (int i = 0; i < numSlots; i++) //adds the time slots from the duration
            {
                slotTime = StartTime.AddMinutes(i * duration);
                sql.CreateClinicSlot(slotDate, slotTime, sClinician, sClinic, sUsername, duration, iPatternID);
            }
        }
        private bool GetIsMonthInPattern(string sMonths, DateTime dDate)
        {
            string monthToCompare = dDate.Month.ToString();
            if (monthToCompare == "10") monthToCompare = "a";
            if (monthToCompare == "11") monthToCompare = "b";
            if (monthToCompare == "12") monthToCompare = "c";

            if(sMonths.Contains(monthToCompare)) return true;

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
            if (intDay == 5) strDay = "Friday";

            FirstDate = dc.GetFirstDateFromList(date, strDay);

            return FirstDate;
        }

    }
}
