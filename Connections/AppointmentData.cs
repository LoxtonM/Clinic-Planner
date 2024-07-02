using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IAppointmentData
    {
        public Appointment GetAppointmentDetails(int refID);
        public List<Appointment> GetAppointments(DateTime dFrom, DateTime dTo, string? clinician, string? clinic);
        public List<Appointment> GetAppointmentsForADay(DateTime clinicDate, string? clinician = null , string? clinic = null);
        public List<Appointment> GetAppointmentsForBWH(DateTime clinicDate);
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
            Appointment appt = _context.Appointments.FirstOrDefault(a => a.RefID == refID);
            return appt;
        }

        public List<Appointment> GetAppointments(DateTime dFrom, DateTime dTo, string? clinician, string? clinic)
        {            
            IQueryable<Appointment> appts = _context.Appointments.Where(a => a.BOOKED_DATE >= dFrom & 
                    a.BOOKED_DATE <= dTo & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional" 
                    & a.Attendance != "Cancelled by patient");

            if (clinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == clinician);
            }
            if (clinic != null)
            {
                appts = appts.Where(l => l.FACILITY == clinic);
            }
            
            return appts.ToList();
        }

        public List<Appointment> GetAppointmentsForADay(DateTime clinicDate, string? clinician = null, string? clinic = null)
        {
            IQueryable<Appointment> appts = _context.Appointments.Where(a => a.BOOKED_DATE == clinicDate 
            & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional"
                    & a.Attendance != "Cancelled by patient");

            if (clinician != null)
            {
                appts = appts.Where(l => l.STAFF_CODE_1 == clinician);
            }
            if (clinic != null)
            {
                appts = appts.Where(l => l.FACILITY == clinic);
            }
            
            return appts.ToList();
        }

        public List<Appointment> GetAppointmentsForBWH(DateTime clinicDate)
        {
            IQueryable<Appointment> appts = _context.Appointments.Where(a => a.BOOKED_DATE == clinicDate
            & a.Attendance != "Declined" & a.Attendance != "Cancelled by professional"
                    & a.Attendance != "Cancelled by patient");

            appts = appts.Where(l => l.FACILITY.Contains("BWH"));
                                 
            return appts.ToList();
        }
    }
}
