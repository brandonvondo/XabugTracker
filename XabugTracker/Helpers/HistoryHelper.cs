using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XabugTracker.Models;

namespace XabugTracker.Helpers
{
    public class HistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserHelper userHelper = new UserHelper();
        public void CreateProjectHistory(int projID, string message)
        {
            var newHistory = new ProjectHistory()
            {
                ProjectId = projID,
                Created = DateTime.Now,
                Message = message,

            };
            db.ProjectHistories.Add(newHistory);
            db.SaveChanges();
        }

        public void ManageHistories(Ticket oldTicket, Ticket newTicket)
        {
            DeveloperUpdate(oldTicket, newTicket);
            PriorityUpdate(oldTicket, newTicket);
            StatusUpdate(oldTicket, newTicket);
            TypeUpdate(oldTicket, newTicket);
            db.SaveChanges();
        }

        private void DeveloperUpdate(Ticket oldTicket, Ticket newTicket)
        {
            var oldDevName = userHelper.GetFullName(oldTicket.DeveloperId);
            var newDevName = userHelper.GetFullName(newTicket.DeveloperId);
            var history = new TicketHistory()
            {
                TicketId = newTicket.Id,
                UserId = HttpContext.Current.User.Identity.GetUserId(),
                Property = "Developer",
                OldValue = oldTicket.DeveloperId == null ? "No Developer Assigned" : oldDevName,
                NewValue = newTicket.DeveloperId == null ? "No Developer Assigned" : newDevName,
                Changedon = DateTime.Now
            };
            db.TicketHistories.Add(history);
        }

        public void DeveloperUpdate(string oldDev, string newDev, int Id)
        {
            var oldDevName = "";
            if (oldDev != null)
            {
                oldDevName = userHelper.GetFullName(oldDev);
            }
            var newDevName = userHelper.GetFullName(newDev);
            var history = new TicketHistory()
            {
                TicketId = Id,
                UserId = HttpContext.Current.User.Identity.GetUserId(),
                Property = "Developer",
                OldValue = oldDev == null ? "No Developer Assigned" : oldDevName,
                NewValue = newDev == null ? "No Developer Assigned" : newDevName,
                Changedon = DateTime.Now
            };
            db.TicketHistories.Add(history);
        }

        public void DeveloperUpdate(string oldDev, string newDev, int Id, string pmId)
        {
            var oldDevName = "";
            if (oldDev != null)
            {
                oldDevName = userHelper.GetFullName(oldDev);
            }
            var newDevName = userHelper.GetFullName(newDev);
            var history = new TicketHistory()
            {
                TicketId = Id,
                UserId = pmId,
                Property = "Developer",
                OldValue = oldDev == null ? "No Developer Assigned" : oldDevName,
                NewValue = newDev == null ? "No Developer Assigned" : newDevName,
                Changedon = DateTime.Now
            };
            db.TicketHistories.Add(history);
        }

        private void PriorityUpdate(Ticket oldTicket, Ticket newTicket)
        {
            var history = new TicketHistory()
            {
                TicketId = newTicket.Id,
                UserId = HttpContext.Current.User.Identity.GetUserId(),
                Property = "Priority",
                OldValue = oldTicket.TicketPriority.Name,
                NewValue = newTicket.TicketPriority.Name,
                Changedon = DateTime.Now
            };
            db.TicketHistories.Add(history);
        }
        private void StatusUpdate(Ticket oldTicket, Ticket newTicket)
        {
            var history = new TicketHistory()
            {
                TicketId = newTicket.Id,
                UserId = HttpContext.Current.User.Identity.GetUserId(),
                Property = "Status",
                OldValue = oldTicket.TicketStatus.Name,
                NewValue = newTicket.TicketStatus.Name,
                Changedon = DateTime.Now
            };
            db.TicketHistories.Add(history);
        }
        private void TypeUpdate(Ticket oldTicket, Ticket newTicket)
        {
            var history = new TicketHistory()
            {
                TicketId = newTicket.Id,
                UserId = HttpContext.Current.User.Identity.GetUserId(),
                Property = "Type",
                OldValue = oldTicket.TicketType.Name,
                NewValue = newTicket.TicketType.Name,
                Changedon = DateTime.Now
            };
            db.TicketHistories.Add(history);
        }

    }
}