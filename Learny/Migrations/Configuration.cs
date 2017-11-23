namespace Learny.Migrations
{
    using Learny.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //
            // required
            //

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var teacherRole = "teacher";
            var studentRole = "student";

            var roleNames = new[] { teacherRole, studentRole };
            foreach (var roleName in roleNames)
            {
                if (context.Roles.Any(r => r.Name == roleName)) continue;

                var role = new IdentityRole { Name = roleName };
                var result = roleManager.Create(role);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }

            var exercise = "�vning";
            var elearning = "E-Learning";
            var lecture = "F�rel�sning";
            var activityTypeNames = new[] { exercise, elearning, lecture };
            var activityTypes = new ActivityType[activityTypeNames.Length];

            for (int i = 0; i < activityTypes.Length; i++)
            {
                activityTypes[i] = new ActivityType { Name = activityTypeNames[i] };
            }

            context.ActivityTypes.AddOrUpdate(
                t => t.Name,
                activityTypes
                );

            //
            // required
            //

            var courses = new Course[]
            {
                new Course { CourseCode = "ND17", Name = ".NET systemutveckling", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) }
            };
            context.Courses.AddOrUpdate(
                c => c.CourseCode,
                courses
                );

            var courseCode = courses[0].CourseCode;
            var courseId = context.Courses.Where(c => c.CourseCode == courseCode).FirstOrDefault().Id;
            var modules = new CourseModule[]
            {
                new CourseModule { Name = "C#", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), CourseId = courseId },
                new CourseModule { Name = "MVC", StartDate = DateTime.Now.AddMonths(1), EndDate = DateTime.Now.AddMonths(2), CourseId = courseId }
            };
            context.Modules.AddOrUpdate(
                c => c.Name,
                modules
                );


            var elearningTypeId = context.ActivityTypes.Where(t => t.Name == elearning).FirstOrDefault().Id;
            var exerciseTypeId = context.ActivityTypes.Where(t => t.Name == exercise).FirstOrDefault().Id;
            var cSharpModuleName = modules[0].Name;
            var cSharpModuleId = context.Modules.Where(m => m.Name == cSharpModuleName).FirstOrDefault().Id;
            var mvcModuleName = modules[1].Name;
            var mvcModuleId = context.Modules.Where(m => m.Name == mvcModuleName).FirstOrDefault().Id;
            var activities = new ModuleActivity[]
            {
                new ModuleActivity { Name = "E-learning", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(15), ActivityTypeId = elearningTypeId, CourseModuleId = cSharpModuleId },
                new ModuleActivity { Name = "�vning", StartDate = DateTime.Now.AddDays(16), EndDate = DateTime.Now.AddMonths(1), ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpModuleId },
                new ModuleActivity { Name = "E-learning", StartDate = DateTime.Now.AddMonths(1), EndDate = DateTime.Now.AddDays(15).AddMonths(1), ActivityTypeId = elearningTypeId, CourseModuleId = mvcModuleId }
            };

            context.Activities.AddOrUpdate(
                a => new { a.Name, a.CourseModuleId },
                activities
                );

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var users = new List<string[]>();
            users.Add(new[] { "teacher@learny.com", "Learny", "Tomas Svensson", teacherRole });
            users.Add(new[] { "student@learny.com", "Learny", "Hans Karlsson", studentRole });

            foreach (var user in users)
            {
                var userUserName = user[0];
                var userEmail = user[0];
                var userPassword = user[1];
                var userName = user[2];
                var userRole = user[3];

                if (context.Users.Any(u => u.UserName == userUserName)) continue;

                var newUser = new ApplicationUser { UserName = userUserName, Email = userEmail, Name = userName };
                if (userRole == studentRole)
                {
                    newUser.CourseId = courseId;
                }

                var result = userManager.Create(newUser, userPassword);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }

                //Add role
                var existingUser = userManager.FindByName(userUserName);
                if (!userManager.IsInRole(existingUser.Id, userRole))
                {
                    userManager.AddToRole(existingUser.Id, userRole);
                }
            }


        }
    }
}
