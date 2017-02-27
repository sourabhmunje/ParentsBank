namespace ParentsBank.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ParentsBank.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }


        protected override void Seed(Models.ApplicationDbContext context)
        {
            // ADD ROLE NAMES IN THIS SECTION. YOU DO NOT HAVE TO USE THE 3 PROVIDED
            // HERE AND CAN USE ROLE NAMES THAT ARE APPROPRIATE FOR YOUR APPLICATION
            var roles = new[]
            {
                 "Admin",
                 "Child",
                 "Parent",
                 "Outsider"
            };

            // USE THE FOLLOWING PATTERN TO ADD DEFAULT USERS TO YOUR SYSTEM
            // ROLES CAN BE COMMA SEPERATED TO ADD MULTIPLE ROLES
            // ROLES PROVIDED MUST EXIST IN THE LIST ABOVE
            var users = new[]
            {
                new {Email = "admin@example.com", Pwd = "Password123", Roles = "Admin"},
                new {Email = "child@example.com", Pwd = "Password123", Roles = "Child"},
                new {Email = "child2@example.com", Pwd = "Password123", Roles = "Child"},
                new {Email = "parent@example.com", Pwd = "Password123", Roles = "Parent"},
                new {Email = "outsider@example.com", Pwd = "Password123", Roles = "Outsider"}
            };

            // DO NOT MODIFY THE CODE BELOW THIS LINE
            roles.ToList().ForEach(r => context.Roles.AddOrUpdate(x => x.Name,
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Id = Guid.NewGuid().ToString(), Name = r }));
            foreach (var user in users)
            {
                ApplicationUserManager mgr = new ApplicationUserManager(
                    new Microsoft.AspNet.Identity.EntityFramework.UserStore<Models.ApplicationUser>(context));
                Models.ApplicationUser existingUser = context.Users.FirstOrDefault(x => x.UserName == user.Email);
                if (existingUser != null) Microsoft.AspNet.Identity.UserManagerExtensions.Delete(mgr, existingUser);
                Models.ApplicationUser au = new Models.ApplicationUser { Email = user.Email, UserName = user.Email };
                var result = mgr.CreateAsync(au, user.Pwd).Result;
                if (!string.IsNullOrEmpty(user.Roles))
                    Microsoft.AspNet.Identity.UserManagerExtensions.AddToRoles(mgr, au.Id, user.Roles.Split(','));
            }

        }
    }
}
