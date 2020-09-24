using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XabugTracker.Models;

namespace XabugTracker.Helpers
{
    public class ProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private Random random = new Random();
        

        public void AddUserToProject(string userId, int projectId )
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project project = db.Projects.Find(projectId);
                var user = db.Users.Find(userId);
                project.Users.Add(user);
                db.SaveChanges();
            }
            
        }

        public Project FindProjectById(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            return project;
        }

        public bool RemoveUserFromProject(string userId, int projectId)
        {
            Project project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            var result = project.Users.Remove(user);
            db.SaveChanges();
            return result;
            
        }

        public ICollection<ApplicationUser> ListUsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }

        public List<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            var resultList = new List<ApplicationUser>();
            foreach (var user in db.Users.ToList())
            {
                if (!IsUserOnProject(user.Id, projectId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public List<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }
        public List<Project> ListUserProjects()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }

        public bool IsUserOnProject(string userId, int projectId)
        {
            Project project = db.Projects.Find(projectId);
            foreach(var member in project.Users)
            {
                if (member.Id == userId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsUserOnAnyProject()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if(user.Projects.FirstOrDefault() != null)
            {
                return true;
            }
            return false;
        }

        public List<Ticket> MyProjectTicketsWithoutDev()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            List<Ticket> ticks = user.Projects.SelectMany(p => p.Tickets.Where(t => t.DeveloperId == null)).ToList();
            return ticks;
        }

        public ApplicationUser UserInRoleNotOnProject(Project project, string roleName)
        {
            List<ApplicationUser> userList = roleHelper.UsersInRole(roleName).ToList();
            List<ApplicationUser> refineList = new List<ApplicationUser>();
            foreach (ApplicationUser user in userList)
            {
                if (!IsUserOnProject(user.Id, project.Id))
                {
                    refineList.Add(user);
                }
            }
            return refineList[random.Next(refineList.Count)];
        }


    }
}