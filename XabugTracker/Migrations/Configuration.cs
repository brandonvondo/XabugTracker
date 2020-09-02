namespace XabugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;
    using XabugTracker.Models;
    using XabugTracker.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<XabugTracker.Models.ApplicationDbContext>
    {
        Random random = new Random();
        UserRolesHelper roleHelper = new UserRolesHelper();
        HistoryHelper historyHelper = new HistoryHelper();
        ProjectHelper projectHelper = new ProjectHelper();
        TicketHelper ticketHelper = new TicketHelper();
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(XabugTracker.Models.ApplicationDbContext context)
        {

            #region Roles
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            #endregion
            #region User Creation
            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));
            var demoPassword = WebConfigurationManager.AppSettings["DemoPassword"];
            var demoDeveloperEmail = WebConfigurationManager.AppSettings["DemoDevEmail"];
            var demoSubmitterEmail = WebConfigurationManager.AppSettings["DemoSubmitterEmail"];
            var demoAdminEmail = WebConfigurationManager.AppSettings["DemoAdminEmail"];
            var demoProjectManagerEmail = WebConfigurationManager.AppSettings["DemoProjectManagerEmail"];
            var defaultAvatarPath = WebConfigurationManager.AppSettings["DefaultAvatarPath"];

            List<string> FirstNames = new List<string>() { "John", "Jacob", "David", "Anya", "Susan", "Joseph", "Jim", "Alex", "PJ", "Abigail", "Chase", "Adriene", "Eddy", "Martel", "Elizabeth", "Patricia", "Scout", "Drew", "Ed", "Ash", "Thomas", "Matt" };
            List<string> LastNames = new List<string>() { "Smith", "Jones", "Brown", "Wilson", "Hill", "Scott", "Cook", "Ross", "Perry", "Powell", "Rivera", "Long", "White", "Garcia", "Gray", "Foster", "Moore", "Campbell", "Walker", "Collins", "Diaz", "Russell", "Griffin" };



            if (!context.Users.Any(u => u.Email == demoAdminEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = demoAdminEmail,
                    AvatarPath = defaultAvatarPath,
                    UserName = demoAdminEmail,
                    FirstName = FirstNames[random.Next(FirstNames.Count)],
                    LastName = LastNames[random.Next(LastNames.Count)]
                }, demoPassword);

                var userID = userManager.FindByEmail(demoAdminEmail).Id;

                userManager.AddToRole(userID, "Admin");
            }

            if (!context.Users.Any(u => u.Email == demoDeveloperEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = demoDeveloperEmail,
                    AvatarPath = defaultAvatarPath,
                    UserName = demoDeveloperEmail,
                    FirstName = FirstNames[random.Next(FirstNames.Count)],
                    LastName = LastNames[random.Next(LastNames.Count)]
                }, demoPassword);

                var userID = userManager.FindByEmail(demoDeveloperEmail).Id;

                userManager.AddToRole(userID, "Developer");
            }

            if (!context.Users.Any(u => u.Email == demoSubmitterEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = demoSubmitterEmail,
                    AvatarPath = defaultAvatarPath,
                    UserName = demoSubmitterEmail,
                    FirstName = FirstNames[random.Next(FirstNames.Count)],
                    LastName = LastNames[random.Next(LastNames.Count)]
                }, demoPassword);

                var userID = userManager.FindByEmail(demoSubmitterEmail).Id;

                userManager.AddToRole(userID, "Submitter");
            }

            if (!context.Users.Any(u => u.Email == demoProjectManagerEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = demoProjectManagerEmail,
                    AvatarPath = defaultAvatarPath,
                    UserName = demoProjectManagerEmail,
                    FirstName = FirstNames[random.Next(FirstNames.Count)],
                    LastName = LastNames[random.Next(LastNames.Count)]
                }, demoPassword);

                var userID = userManager.FindByEmail(demoProjectManagerEmail).Id;

                userManager.AddToRole(userID, "Project Manager");
            }

            //40 Random Users
            for (int i = 0; i < 40; i++)
            {
                var email = $"xabuguser{i}@mailinator.com";
                if (!context.Users.Any(u => u.Email == email))
                {
                    var fn = FirstNames[random.Next(FirstNames.Count)];
                    var ln = LastNames[random.Next(LastNames.Count)];
                    var forlooppw = "PasswordPassword";
                    var role = "";

                    if (i <= 2)
                    {
                        role = "Admin";
                    }
                    else if (i > 2 && i <= 12)
                    {
                        role = "Project Manager";
                    }
                    else if (i > 12 && i <= 24)
                    {
                        role = "Submitter";
                    }
                    else if (i > 24 && i <= 40)
                    {
                        role = "Developer";
                    }

                    userManager.Create(new ApplicationUser()
                    {
                        Email = email,
                        AvatarPath = defaultAvatarPath,
                        UserName = email,
                        FirstName = fn,
                        LastName = ln
                    }, forlooppw);

                    var userID = userManager.FindByEmail(email).Id;

                    userManager.AddToRole(userID, role);
                }
            }
            #endregion
            context.SaveChanges();
            #region Ticket Types
            context.TicketTypes.AddOrUpdate(
                tt => tt.Name,
                new TicketType() { Name = "Software" },
                new TicketType() { Name = "Feature Request" },
                new TicketType() { Name = "UI" },
                new TicketType() { Name = "Defect" },
                new TicketType() { Name = "Other" }
                );
            #endregion
            #region Ticket Priorities
            context.TicketPriorities.AddOrUpdate(
                tp => tp.Name,
                new TicketPriority() { Name = "Low" },
                new TicketPriority() { Name = "Medium" },
                new TicketPriority() { Name = "High" },
                new TicketPriority() { Name = "On Hold" }
                );
            #endregion
            #region Ticket Statuses
            context.TicketStatuses.AddOrUpdate(
                ts => ts.Name,
                new TicketStatus() { Name = "Open" },
                new TicketStatus() { Name = "Assigned" },
                new TicketStatus() { Name = "Resolved" },
                new TicketStatus() { Name = "Reopened" },
                new TicketStatus() { Name = "Archived" }
                );
            #endregion
            context.SaveChanges();

            #region Project Seed
            context.Projects.AddOrUpdate(
                p => p.Name,
                new Project() { Name = "Seed 1", Created = DateTime.Now.AddDays(-60) },
                new Project() { Name = "Seed 2", Created = DateTime.Now.AddDays(-45) },
                new Project() { Name = "Seed 3", Created = DateTime.Now.AddDays(-30) },
                new Project() { Name = "Seed 4", Created = DateTime.Now.AddDays(-15) },
                new Project() { Name = "Seed 5", Created = DateTime.Now }
                );
            #endregion
            context.SaveChanges();

            List<ApplicationUser> adminList = roleHelper.UsersInRole("Admin").ToList();
            List<ApplicationUser> developerList = roleHelper.UsersInRole("Developer").ToList();
            List<ApplicationUser> submitterList = roleHelper.UsersInRole("Submitter").ToList();
            List<ApplicationUser> pmList = roleHelper.UsersInRole("Project Manager").ToList();
            List<Project> projList = context.Projects.ToList();


            context.SaveChanges();

            #region Assigning Users to Projects by Role
            foreach (var project in projList)
            {
                if (project.Users.Count < 11)
                {
                    List<ApplicationUser> innerListP = pmList.Where(prom => prom.Projects.Count == 0).ToList();
                    var pm = innerListP[random.Next(innerListP.Count)];
                    projectHelper.AddUserToProject(pm.Id, project.Id);
                    string PHmessagem = "Project Manager " + pm.FullName + " has been assigned to be the project manager";
                    project.ManagerId = pm.Id;
                    historyHelper.CreateProjectHistory(project.Id, PHmessagem);

                    foreach (var admin in adminList)
                    {
                        projectHelper.AddUserToProject(admin.Id, project.Id);
                        string PHmessageA = admin.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                        historyHelper.CreateProjectHistory(project.Id, PHmessageA);
                    }

                    List<ApplicationUser> innerListS = submitterList.Where(sub => sub.Projects.Contains(project) == false).ToList();
                    var user1s = innerListS[random.Next(innerListS.Count)];
                    projectHelper.AddUserToProject(user1s.Id, project.Id);
                    innerListS = submitterList.Where(sub => sub.Projects.Contains(project) == false).ToList();
                    var user2s = innerListS[random.Next(innerListS.Count)];
                    projectHelper.AddUserToProject(user2s.Id, project.Id);
                    string PHmessageS = user1s.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(project.Id, PHmessageS);
                    PHmessageS = user2s.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(project.Id, PHmessageS);
                    historyHelper.CreateProjectHistory(project.Id, PHmessageS);

                    List<ApplicationUser> innerListD = developerList.Where(dev => dev.Projects.Contains(project) == false).ToList();
                    var user = innerListD[random.Next(innerListD.Count)];
                    innerListD = developerList.Where(dev => dev.Projects.Contains(project) == false).ToList();
                    var user2 = innerListD[random.Next(innerListD.Count)];
                    innerListD = developerList.Where(dev => dev.Projects.Contains(project) == false).ToList();
                    var user3 = innerListD[random.Next(innerListD.Count)];
                    innerListD = developerList.Where(dev => dev.Projects.Contains(project) == false).ToList();
                    var user4 = innerListD[random.Next(innerListD.Count)];
                    projectHelper.AddUserToProject(user.Id, project.Id);
                    projectHelper.AddUserToProject(user2.Id, project.Id);
                    projectHelper.AddUserToProject(user3.Id, project.Id);
                    projectHelper.AddUserToProject(user4.Id, project.Id);
                    string PHmessageD = user.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(project.Id, PHmessageD);
                    PHmessageD = user2.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(project.Id, PHmessageD);
                    PHmessageD = user3.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(project.Id, PHmessageD);
                    PHmessageD = user4.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(project.Id, PHmessageD);
                }
            }
            #endregion

            projList = context.Projects.Include("Users").ToList();

            #region Ticket Creation
            foreach (var project in projList)
            {
                List<ApplicationUser> projectUsers = project.Users.ToList();

                if (projectUsers.Count == 0)
                {
                    throw new Exception();
                }

                List<ApplicationUser> pmListOP = roleHelper.ProjectUsersInRole("Project Manager", projectUsers);
                List<ApplicationUser> submitterListOP = roleHelper.ProjectUsersInRole("Submitter", projectUsers);
                List<ApplicationUser> developerListOP = roleHelper.ProjectUsersInRole("Developer", projectUsers);

                foreach (var type in context.TicketTypes.ToList())
                {
                    foreach (var status in context.TicketStatuses.ToList())
                    {
                        foreach (var priority in context.TicketPriorities.ToList())
                        {
                            if (status.Name != "Archived")
                            {
                                var subm = submitterListOP[random.Next(submitterListOP.Count)];
                                var dev = developerListOP[random.Next(developerListOP.Count)];
                                var devId = dev.Id;

                                var resolved = false;
                                if (status.Name == "Resolved")
                                {
                                    resolved = true;
                                }
                                string issueStr = $"{project.Name} Seeded Ticket: {type.Name}, {priority.Name}, {status.Name}";
                                string issuebodyStr = $"This is a seeded ticket of type '{type.Name}' with a priority of '{priority.Name}' with the status of '{status.Name}' assigned to '{dev.FullName}' created by '{subm.FullName}'";
                                if (status.Name == "Open")
                                {
                                    devId = null;
                                    issuebodyStr = $"This is a seeded ticket of type '{type.Name}' with a priority of '{priority.Name}' with the status of '{status.Name}' submitted by '{subm.FullName}'";
                                }

                                var newTicket = new Ticket()
                                {
                                    ProjectId = project.Id,
                                    TicketPriorityId = priority.Id,
                                    TicketStatusId = status.Id,
                                    TicketTypeId = type.Id,
                                    DeveloperId = devId,
                                    SubmitterId = subm.Id,
                                    Created = DateTime.Now,
                                    Issue = issueStr,
                                    IssueDescription = issuebodyStr,
                                    IsResolved = resolved,
                                    IsArchived = false
                                };
                                context.Tickets.Add(newTicket);
                                context.SaveChanges();
                                if (status.Name != "Open")
                                {
                                    ticketHelper.ManageTicketNotifications(null, devId, newTicket.Id);
                                    historyHelper.DeveloperUpdate(null, devId, newTicket.Id, project.ManagerId);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}
