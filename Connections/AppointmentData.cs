using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IAppointmentData
    {
        public Appointment GetAppointmentDetails(int refID);
        public IEnumerable<Appointment> GetAppointments(DateTime dFrom, DateTime dTo, string? clinician, string? clinic);
        public IEnumerable<Appointment> GetAppointmentsForADay(DateTime clinicDate, string? clinician, string? clinic);
        public IEnumerable<Appointment> GetAppointmentsForBWH(DateTime clinicDate);
    }
    public class AppointmentData : IAppointmentData
    {
        private readonly DataContext _context;
        public AppointmentData(DataContext context)
        {
            _context = context;
        }
       

        public Appointment GetAppointmentDetails(int refID)
        {
            var appt = _context.Appointments.FirstOrDefault(a => a.RefID == refID);
            return appt;
        }

        public IEnumerable<Appointment> GetAppointments(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
        {            
            var appts = _context.Appointments.Where(a => a.BOOKED_DATE >= dFrom & 
                    a.BOOKED_DATE <= dTo & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional" 
                    & a.Attendance != "Cancelled by patient").ToList();

            if (clinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == clinician).ToList();
            }
            if (clinic != null)
            {
                appts = appts.Where(l => l.FACILITY == clinic).ToList();
            }
            
            return appts;
        }

        public IEnumerable<Appointment> GetAppointmentsForADay(DateTime clinicDate, string? clinician, string? clinic)
        {
            var appts = _context.Appointments.Where(a => a.BOOKED_DATE == clinicDate 
            & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional"
                    & a.Attendance != "Cancelled by patient").ToList();

            if (clinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == clinician).ToList();
            }
            if (clinic != null)
            {
                appts = appts.Where(l => l.FACILITY == clinic).ToList();
            }
            
            return appts;
        }

        public IEnumerable<Appointment> GetAppointmentsForBWH(DateTime clinicDate)
        {
            var appts = _context.Appointments.Where(a => a.BOOKED_DATE == clinicDate
            & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional"
                    & a.Attendance != "Cancelled by patient").ToList();

            appts = appts.Where(l => l.FACILITY.Contains("BWH")).ToList();
                                 
            return appts;
        }
    }
}
