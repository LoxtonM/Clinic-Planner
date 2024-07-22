using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IAlertsData
    {
        public List<Alerts> GetAlerts(int mpi);
        public Alerts GetAlertDetails(int id);
    }
    public class AlertsData : IAlertsData
    {
        private readonly DataContext _context;
        public AlertsData(DataContext context)
        {
            _context = context;
        }        

        public List<Alerts> GetAlerts(int mpi)
        {
            IQueryable<Alerts> alerts = _context.Alerts.Where(a => a.MPI == mpi & a.EffectiveToDate == null);
            
            return alerts.ToList();
        }

        public Alerts GetAlertDetails(int id)
        {
            Alerts alert = _context.Alerts.FirstOrDefault(a => a.AlertID == id);

            return alert;
        }

        
    }
}
