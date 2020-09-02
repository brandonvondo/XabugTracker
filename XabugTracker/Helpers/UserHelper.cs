using XabugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace XabugTracker.Helpers

{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string GetAvatarPath()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return user.AvatarPath;
        }

        public string GetUserId()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return userId;
        }

        public ApplicationUser GetUser()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return user;
        }

        public ApplicationUser GetUserById(string userId)
        {
            var user = db.Users.Find(userId);
            return user;
        }

        public string GetFullName()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            return firstName + " " + lastName;
        }
        public string GetFullName(string userId)
        {
            var user = db.Users.Find(userId);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            return firstName + " " + lastName;
        }

        public string LastNameFirst()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return $"{user.LastName}, ${user.FirstName}";
        }

        public List<Ticket> MyCreatedTickets()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var count = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
            return count;
        }

        public List<Ticket> MyAssignedTickets()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var count = db.Tickets.Where(t => t.DeveloperId == userId).ToList();
            return count;
        }
    }
}
