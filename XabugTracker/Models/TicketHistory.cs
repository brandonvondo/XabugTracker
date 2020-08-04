using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XabugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        #region Parents/Children
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        #endregion

        #region Actual Properties
        //The property of the ticket that was changed
        public string Property { get; set; }
        //The original property
        public string OldValue { get; set; }
        //The new property
        public string NewValue { get; set; }
        public DateTime Changedon { get; set; }
        #endregion
    }
}