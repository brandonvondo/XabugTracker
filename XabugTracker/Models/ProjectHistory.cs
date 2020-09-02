using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XabugTracker.Models
{
    public class ProjectHistory
    {
        public int Id { get; set; }

        #region Parents/Children
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        #endregion

        #region Actual Property
        public string Message { get; set; }
        public DateTime Created { get; set; }
        #endregion
    }
}