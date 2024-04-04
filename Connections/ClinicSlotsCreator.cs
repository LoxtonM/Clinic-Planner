using CPTest.Data;

namespace CPTest.Connections
{
    public class ClinicSlotsCreator
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly DataConnections _dc;
        private readonly SqlServices _sql;

        public ClinicSlotsCreator(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _dc = new DataConnections(_context);
            _sql = new SqlServices(_config);
        }
        public void SetupClinicPattern(string clinician, string clinic, DateTime startDate, DateTime? endDate,
            int startHr, int startMin, int numSlots, int duration, int dayOfWeek, int weekOfMonth, 
            string monthOfYear, string username) //creates slots for a standard clinic pattern
        {
            _sql.SaveClinicPattern(clinician, clinic, dayOfWeek, weekOfMonth, monthOfYear, numSlots, duration,
                startHr, startMin, startDate, endDate);

            int iPatternID = _context.ClinicPattern.FirstOrDefault(p => p.StaffID == clinician && p.Clinic == clinic && 
                                p.DyOfWk == dayOfWeek && p.WkOfMth == weekOfMonth && p.startDate == startDate && p.MthOfYr == monthOfYear
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

            if(endDate == DateTime.Parse("0001-01-01")) //set a default end date if none specified
            {
                endDate = DateTime.Now.AddYears(1);
            }

            if (GetIsMonthInPattern(monthOfYear, firstDayCurrentMonth)) //check if month is in the pattern
            {
                firstOccurrenceCurrentMonth = FindFirstOccurrence(firstDayCurrentMonth, dayOfWeek);
                firstDateCurrentMonth = firstOccurrenceCurrentMonth.AddDays(-7 + (weekOfMonth * 7));
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
            if (GetIsMonthInPattern(monthOfYear, firstDayNextMonth))
            {
                firstOccurrenceNextMonth = FindFirstOccurrence(firstDayNextMonth, dayOfWeek);
                firstDateNextMonth = firstOccurrenceNextMonth.AddDays(-7 + (weekOfMonth * 7));
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

            if(GetIsMonthInPattern(monthOfYear, nextClinicDate) && nextClinicDate >= startDate && !_dc.GetIsNationalHolday(nextClinicDate))
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
                if (GetIsMonthInPattern(monthOfYear, firstDayNextMonth))
                {
                    firstOccurrenceNextMonth = FindFirstOccurrence(firstDayNextMonth, dayOfWeek);
                    firstDateNextMonth = firstOccurrenceNextMonth.AddDays(7 - (weekOfMonth * 7));                //DateAdd("ww", intWeek - 1, firstOccurrenceNextMonth)
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
                CreateDayOfClinicSlots(clinician, clinic, date, slotTime, numSlots, duration, username, iPatternID);
            }
        }

        public void SetupAdHocClinic(DateTime clinicDate, int startHr, int startMin, string clinicianID,
        string clinicID, int duration, int numSlots, string staffCode)
        {
            DateTime dStartTime = DateTime.Parse("1899-12-30 " + startHr.ToString() + ":" + startMin.ToString() + ":00");

            _sql.SaveAdHocClinic(clinicianID, clinicID, numSlots, duration, startHr, startMin, clinicDate);

            int iPatternID = _context.ClinicsAdded.FirstOrDefault(p => p.ClinicianID == clinicianID && p.ClinicID == clinicID &&
                                p.ClinicDate == clinicDate && p.StartHr == startHr && p.StartMin == startMin).ID;

            CreateDayOfClinicSlots(clinicianID, clinicID, clinicDate, dStartTime, numSlots, duration, staffCode, iPatternID);
        }

        private void CreateDayOfClinicSlots(string clinician, string clinic, DateTime slotDate,
            DateTime StartTime, int numSlots, int duration, string sUsername, int iPatternID)
        {
            DateTime slotTime;

            for (int i = 0; i < numSlots; i++) //adds the time slots from the duration
            {
                slotTime = StartTime.AddMinutes(i * duration);
                _sql.CreateClinicSlot(slotDate, slotTime, clinician, clinic, sUsername, duration, iPatternID);
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

    }
}
