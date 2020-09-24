using XabugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Data.Entity;

namespace XabugTracker.Helpers

{
    public class TicketHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projectHelper = new ProjectHelper();
        private UserRolesHelper userRolesHelper = new UserRolesHelper();

        public void ManageTicketNotifications(Ticket oldTicket, Ticket newTicket)
        {
            if(oldTicket.DeveloperId != newTicket.DeveloperId && newTicket.DeveloperId != null)
            {
                var newNotification = new TicketNotification()
                {
                    TicketId = newTicket.Id,
                    UserId = newTicket.DeveloperId,
                    Created = DateTime.Now,
                    Message = $"You have been assigned a Ticket: ID({newTicket.Id}) '{newTicket.Issue}' on Project'{newTicket.Project.Name}'",

                };

                db.TicketNotifications.Add(newNotification);
                db.SaveChanges();
            }

            if (oldTicket.DeveloperId != newTicket.DeveloperId && newTicket.DeveloperId == null)
            {
                var newNotification = new TicketNotification()
                {
                    TicketId = newTicket.Id,
                    UserId = oldTicket.DeveloperId,
                    Created = DateTime.Now,
                    Message = $"You have been removed on Ticket: ID({newTicket.Id}) '{newTicket.Issue}' on Project '{newTicket.Project.Name}'",

                };

                db.TicketNotifications.Add(newNotification);
                db.SaveChanges();
            }

            if (oldTicket.DeveloperId != newTicket.DeveloperId && oldTicket.DeveloperId != null && newTicket.DeveloperId != null)
            {
                var newNotification = new TicketNotification()
                {
                    TicketId = newTicket.Id,
                    UserId = oldTicket.DeveloperId,
                    Created = DateTime.Now,
                    Message = $"You have been replaced on Ticket: ID({newTicket.Id}) '{newTicket.Issue}' on Project '{newTicket.Project.Name}'",

                };

                db.TicketNotifications.Add(newNotification);
                db.SaveChanges();
            }
        }

        public void ManageTicketNotifications(string oldDev, string newDev, int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            if (oldDev != newDev && newDev != null)
            {
                var newNotification = new TicketNotification()
                {
                    TicketId = ticketId,
                    UserId = newDev,
                    Created = DateTime.Now,
                    Message = $"You have been assigned a Ticket: ID({ticketId}) '{ticket.Issue}' on Project '{ticket.Project.Name}'",

                };

                db.TicketNotifications.Add(newNotification);
                db.SaveChanges();
            }

            if (oldDev != newDev && newDev == null)
            {
                var newNotification = new TicketNotification()
                {
                    TicketId = ticketId,
                    UserId = oldDev,
                    Created = DateTime.Now,
                    Message = $"You have been removed on Ticket: ID({ticketId}) '{ticket.Issue}' on Project '{ticket.Project.Name}'",

                };

                db.TicketNotifications.Add(newNotification);
                db.SaveChanges();
            }

            if (oldDev != newDev && oldDev != null && newDev != null)
            {
                var newNotification = new TicketNotification()
                {
                    TicketId = ticketId,
                    UserId = oldDev,
                    Created = DateTime.Now,
                    Message = $"You have been replaced on Ticket: ID({ticketId}) '{ticket.Issue}' on Project '{ticket.Project.Name}'",

                };

                db.TicketNotifications.Add(newNotification);
                db.SaveChanges();
            }
        }

        public TicketNotification GetTicketNotification(int id)
        {
            TicketNotification ticketNotification = db.TicketNotifications.Find(id);
            return ticketNotification;
        }

        public void DeleteNotification(int id)
        {
            TicketNotification ticketNotification = db.TicketNotifications.Find(id);
            db.TicketNotifications.Remove(ticketNotification);
            db.SaveChanges();
        }

        public void ReadNotification(int id)
        {
            TicketNotification ticketNotification = db.TicketNotifications.Find(id);
            ticketNotification.IsRead = true;
            db.Entry(ticketNotification).State = EntityState.Modified;
            db.SaveChanges();
        }


        public List<Ticket> ListUserTickets(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            List<Ticket> tickets = new List<Ticket>();
            foreach (var project in projects)
            {
                var ticketsL = project.Tickets.ToList();
                foreach (var ticket in ticketsL)
                {
                    tickets.Add(ticket);
                }
            }
            return (tickets);
        }

        public bool CanUserInteract(int ticketId)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            Ticket ticket = db.Tickets.Find(ticketId);
            bool isUserOn = projectHelper.IsUserOnProject(userId, ticket.ProjectId);
            if ((userId == ticket.SubmitterId) || (userId == ticket.DeveloperId) || (isUserOn))
                {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int TicketCount()
        {
            var ticketC = db.Tickets.ToList().Count;
            return ticketC;
        }

        public decimal[] TypeGraph()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var list = ListUserTickets(userId);
            decimal software = 0;
            decimal feature = 0;
            decimal UI = 0;
            decimal defect = 0;
            decimal other = 0;
            foreach (var ticket in list)
            {
                switch(ticket.TicketType.Name)
                {
                    case "Software": software++;
                        break;
                    case "Feature Request": feature++;
                        break;
                    case "UI": UI++;
                        break;
                    case "Defect": defect++;
                        break;
                    case "Other": other++;
                        break;
                }
            }

            decimal[] result = { software, feature, UI, defect, other };

            return result;
        }

        public decimal[] PriorityGraph()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var list = ListUserTickets(userId);
            decimal low = 0;
            decimal medium = 0;
            decimal high = 0;
            decimal onhold = 0;
            foreach (var ticket in list)
            {
                switch (ticket.TicketPriority.Name)
                {
                    case "Low":
                        low++;
                        break;
                    case "Medium":
                        medium++;
                        break;
                    case "High":
                        high++;
                        break;
                    case "On Hold":
                        onhold++;
                        break;
                }
            }

            decimal[] result = { onhold, medium, high, low };

            return result;
        }

        public bool graphShow()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var list = ListUserTickets(userId);
            if (list.Count > 0)
            {
                return true;
            }

            return false;
        }

        public SelectList LayoutViewbag(string TicketWhat)
        {
            switch(TicketWhat)
            {
                case "prio": return new SelectList(db.TicketPriorities, "Id", "Name");
                case "type": return new SelectList(db.TicketTypes, "Id", "Name");
            }
            return null;
        }
    }
}
