namespace XabugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using XabugTracker.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<XabugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(XabugTracker.Models.ApplicationDbContext context)
        {
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

            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "xabvstudios@gmail.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "xabvstudios@gmail.com",
                    UserName = "xabvstudios@gmail.com",
                    FirstName = "Brandon",
                    LastName = "Von Dolteren",
                    AvatarPath = "test0.png"
                }, "1!Qa");

                var userID = userManager.FindByEmail("xabvstudios@gmail.com").Id;

                userManager.AddToRole(userID, "Admin");
            }

            if (!context.Users.Any(u => u.Email == "submitter@mail.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "submitter@mail.com",
                    UserName = "submitter@mail.com",
                    FirstName = "Mitt",
                    LastName = "Subber",
                    AvatarPath = "test1.png"
                }, "1!Qa");

                var userID = userManager.FindByEmail("submitter@mail.com").Id;

                userManager.AddToRole(userID, "Submitter");
            }

            if (!context.Users.Any(u => u.Email == "projectmanager@mail.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "projectmanager@mail.com",
                    UserName = "projectmanager@mail.com",
                    FirstName = "Proman",
                    LastName = "Ager",
                    AvatarPath = "test2.png"
                }, "1!Qa");

                var userID = userManager.FindByEmail("projectmanager@mail.com").Id;

                userManager.AddToRole(userID, "Project Manager");
            }

            if (!context.Users.Any(u => u.Email == "developer@mail.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "developer@mail.com",
                    UserName = "developer@mail.com",
                    FirstName = "Devon",
                    LastName = "Eloper",
                    AvatarPath = "test3.png"
                }, "1!Qa");

                var userID = userManager.FindByEmail("developer@mail.com").Id;

                userManager.AddToRole(userID, "Developer");
            }
        }
    }
}
