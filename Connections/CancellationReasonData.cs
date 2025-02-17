﻿using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface ICancellationReasonData
    {
        public List<CancellationReason> GetCancellationReasonsList();
    }
    public class CancellationReasonData : ICancellationReasonData
    {
        private readonly CPXContext _context;
        public CancellationReasonData(CPXContext context)
        {
            _context = context;
        }
       
        public List<CancellationReason> GetCancellationReasonsList() 
        {
            IQueryable<CancellationReason> cr = _context.CancellationReasons;

            return cr.ToList();
        }
    }
}
