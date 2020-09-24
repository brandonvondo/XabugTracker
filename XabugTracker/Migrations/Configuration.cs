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
        NameMaker nm = new NameMaker();
        SeedHelper seedHelper = new SeedHelper();
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




            if (!context.Users.Any(u => u.Email == demoAdminEmail))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = demoAdminEmail,
                    AvatarPath = defaultAvatarPath,
                    UserName = demoAdminEmail,
                    FirstName = nm.FirstNameGenerate(),
                    LastName = nm.LastNameGenerate()
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
                    FirstName = nm.FirstNameGenerate(),
                    LastName = nm.LastNameGenerate()
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
                    FirstName = nm.FirstNameGenerate(),
                    LastName = nm.LastNameGenerate()
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
                    FirstName = nm.FirstNameGenerate(),
                    LastName = nm.LastNameGenerate()
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
                    var fn = nm.FirstNameGenerate();
                    var ln = nm.LastNameGenerate();
                    var forlooppw = demoPassword;
                    var role = "";

                    if (i <= 2)
                    {
                        role = "Admin";
                    }
                    else if (i > 2 && i <= 6)
                    {
                        role = "Project Manager";
                    }
                    else if (i > 6 && i <= 18)
                    {
                        role = "Submitter";
                    }
                    else if (i > 18 && i <= 40)
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
            bool subPro = false;
            bool devPro = false;
            bool pmPro = false;
            var demosub = context.Users.Where(u => u.Email == demoSubmitterEmail).FirstOrDefault();
            var demodev = context.Users.Where(u => u.Email == demoDeveloperEmail).FirstOrDefault();
            var demopm = context.Users.Where(u => u.Email == demoProjectManagerEmail).FirstOrDefault();
            foreach (var project in projList)
            {
                var pmV = 0;
                var s = 0;
                var d = 0;
                var adminBool = false;
                if (project.Users.Count < 11)
                {
                    for (var m = 0; m < 4; m++)
                    {
                        if (pmV < 1)
                        {
                            List<ApplicationUser> innerListP = pmList.Where(prom => prom.Projects.Count == 0).ToList();
                            var pm = innerListP[random.Next(innerListP.Count)];
                            if (!pmPro)
                            {
                                projectHelper.AddUserToProject(demopm.Id, project.Id);
                                pmPro = true;
                            }
                            else
                            {

                                projectHelper.AddUserToProject(pm.Id, project.Id);
                            }
                            string PHmessage = "Project Manager " + pm.FullName + " has been assigned to be the project manager";
                            project.ManagerId = pm.Id;
                            historyHelper.CreateProjectHistory(project.Id, PHmessage);
                            pmV++;
                        }
                        if (!adminBool)
                        {
                            foreach (var user in adminList)
                            {
                                projectHelper.AddUserToProject(user.Id, project.Id);
                                string PHmessage = user.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                                historyHelper.CreateProjectHistory(project.Id, PHmessage);
                                adminBool = true;
                            }
                        }
                        if (s < 2)
                        {
                            var user = projectHelper.UserInRoleNotOnProject(project, "Submitter");
                            if (!subPro)
                            {
                                projectHelper.AddUserToProject(demosub.Id, project.Id);
                                subPro = true;
                            }
                            else
                            {

                                projectHelper.AddUserToProject(user.Id, project.Id);
                            }
                            string PHmessage = user.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                            historyHelper.CreateProjectHistory(project.Id, PHmessage);
                            s++;
                        }
                        if (d < 4)
                        {
                            var user = projectHelper.UserInRoleNotOnProject(project, "Developer");
                            if (!devPro)
                            {
                                projectHelper.AddUserToProject(demodev.Id, project.Id);
                                devPro = true;
                            }
                            else
                            {

                                projectHelper.AddUserToProject(user.Id, project.Id);
                            }
                            string PHmessage = user.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                            historyHelper.CreateProjectHistory(project.Id, PHmessage);
                            d++;
                        }
                    }
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
