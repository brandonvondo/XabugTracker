using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XabugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        #region Parents/Children
        public virtual ICollection<Ticket> Tickets { get; set; }
        public string ManagerId { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<ProjectHistory> PHistory { get; set; }
        #endregion

        #region Actual Properties
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public bool IsArchived { get; set; }
        #endregion

        #region Constructor
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
            PHistory = new HashSet<ProjectHistory>();
        }
        #endregion

    }
}