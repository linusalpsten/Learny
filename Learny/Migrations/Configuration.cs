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
    using Learny.Settings;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            // THIS CODE IS FOR DEBUGGING Migrations

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
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

            #region Required
            //
            // required
            //

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);


            var roleNames = new[] { RoleName.teacher, RoleName.student };
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

            var activityTypeNames = new string[,] {
                { ActivityTypeName.exercise, ActivityTypeName.exerciseShortName},
                { ActivityTypeName.elearning, ActivityTypeName.elearningShortName},
                { ActivityTypeName.lecture, ActivityTypeName.lectureShortName}
            };
            var activityTypes = new ActivityType[activityTypeNames.GetUpperBound(0)+1];

            for (int i = 0; i < activityTypes.Length; i++)
            {
                activityTypes[i] = new ActivityType { Name = activityTypeNames[i, 0], ShortName = activityTypeNames[i, 1] };
            }

            context.ActivityTypes.AddOrUpdate(
                t => t.Name,
                activityTypes
                );
            context.SaveChanges();

            //
            // required
            //

            #endregion

            #region example data

            var courseCode = "ND17";
            var startDate = new DateTime(2017, 08, 28);


            var courses = new Course[]
            {
                new Course { CourseCode = courseCode, Name = ".NET systemutveckling",
                    StartDate = startDate, EndDate = startDate.AddMonths(4) }
            };
            context.Courses.AddOrUpdate(
                c => c.CourseCode,
                courses
                );

            context.SaveChanges();

            var courseId = context.Courses.Where(c => c.CourseCode == courseCode).FirstOrDefault().Id;
            var modules = new CourseModule[]
            {
                new CourseModule { Name = "C#", StartDate = startDate.AddDays(0),
                    EndDate = startDate.AddDays(29), CourseId = courseId },
                new CourseModule { Name = "Testning", StartDate = startDate.AddDays(22),
                    EndDate = startDate.AddDays(68), CourseId = courseId },
                new CourseModule { Name = "Webb", StartDate = startDate.AddDays(30),
                    EndDate = startDate.AddDays(53), CourseId = courseId },
                new CourseModule { Name = "MVC", StartDate = startDate.AddDays(45),
                    EndDate = startDate.AddDays(62), CourseId = courseId },
                new CourseModule { Name = "Databas", StartDate = startDate.AddDays(63),
                    EndDate = startDate.AddDays(73), CourseId = courseId },
                new CourseModule { Name = "App. Utv.", StartDate = startDate.AddDays(74),
                    EndDate =startDate.AddDays(82), CourseId = courseId },
                new CourseModule { Name = "MVC fördj", StartDate = startDate.AddDays(81),
                    EndDate =startDate.AddDays(111), CourseId = courseId }
            };
            context.Modules.AddOrUpdate(
                c => new { c.Name, c.StartDate },
                modules
                );

            context.SaveChanges();

            var elearningTypeId = context.ActivityTypes.Where(t => t.Name == ActivityTypeName.elearning).FirstOrDefault().Id;
            var exerciseTypeId = context.ActivityTypes.Where(t => t.Name == ActivityTypeName.exercise).FirstOrDefault().Id;
            var lectureTypeId = context.ActivityTypes.Where(t => t.Name == ActivityTypeName.lecture).FirstOrDefault().Id;
            var cSharpModuleName1 = modules[0].Name;
            var cSharpModuleStartDate1 = modules[0].StartDate;
            var cSharpModuleId1 = context.Modules.Where(m => m.Name == cSharpModuleName1 && m.StartDate == cSharpModuleStartDate1).FirstOrDefault().Id;
            var testModuleName1 = modules[1].Name;
            var testModuleStartDate1 = modules[1].StartDate;
            var testModuleId1 = context.Modules.Where(m => m.Name == testModuleName1 && m.StartDate == testModuleStartDate1).FirstOrDefault().Id;

            var mvcAdvanced = modules[6].Name;
            var mvcAdvancedStartDate = modules[6].StartDate;
            var mvcAdvancedId = context.Modules.Where(m => m.Name == mvcAdvanced && m.StartDate == mvcAdvancedStartDate).FirstOrDefault().Id;

            var activities = new ModuleActivity[]
            {
                new ModuleActivity {
                    Name = "Intro + Kapitel 1.1, 1.2", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(0), EndDate = startDate.AddDays(0),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Kapitel 1.3", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(1), EndDate = startDate.AddDays(1),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Kapitel 1.4 + 1.5", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(1), EndDate = startDate.AddDays(1),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "C# Intro", Description ="Adrian",
                    StartDate = startDate.AddDays(2), EndDate = startDate.AddDays(2),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Övning 2", Description ="Flow Control",
                    StartDate = startDate.AddDays(3), EndDate = startDate.AddDays(3),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "C# Grund", Description ="Adrian",
                    StartDate = startDate.AddDays(4), EndDate = startDate.AddDays(4),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Kapitel 1.6 + 1.7", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(7), EndDate = startDate.AddDays(7),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Kapitel 1.8", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(7), EndDate = startDate.AddDays(7),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Kapitel 1.7 + 1.8", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(8), EndDate = startDate.AddDays(8),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Övning 2", Description ="Flow Control",
                    StartDate = startDate.AddDays(8), EndDate = startDate.AddDays(8),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Repition", Description ="",
                    StartDate = startDate.AddDays(8), EndDate = startDate.AddDays(8),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "OOP", Description ="Adrian",
                    StartDate = startDate.AddDays(9), EndDate = startDate.AddDays(9),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Övning 3", Description =" Inkapsling, arv och polymorfism Nytt utkast",
                    StartDate = startDate.AddDays(10), EndDate = startDate.AddDays(10),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "OOP 2", Description ="Adrian",
                    StartDate = startDate.AddDays(11), EndDate = startDate.AddDays(11),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Generics", Description ="Adrian",
                    StartDate = startDate.AddDays(21), EndDate = startDate.AddDays(21),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "LINQ", Description ="Adrian",
                    StartDate = startDate.AddDays(21), EndDate = startDate.AddDays(21),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpModuleId1 },

                new ModuleActivity {
                    Name = "Unit Test", Description ="C# Best Practices: Improving on the Basics med Deborah Kurata",
                    StartDate = startDate.AddDays(22), EndDate = startDate.AddDays(22),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = testModuleId1 },

                new ModuleActivity {
                    Name = "Projekt", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(107), EndDate = startDate.AddDays(108),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvancedId },

                new ModuleActivity {
                    Name = "Sprint review", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(109), EndDate = startDate.AddDays(109),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvancedId },

                new ModuleActivity {
                    Name = "Projekt", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(110), EndDate = startDate.AddDays(110),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvancedId },

                new ModuleActivity {
                    Name = "Slutredovisning", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(110), EndDate = startDate.AddDays(110),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvancedId },

                new ModuleActivity {
                    Name = "Avslutning", Description ="",
                    StartDate = startDate.AddDays(111), EndDate = startDate.AddDays(111),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvancedId },

            };

            context.Activities.AddOrUpdate(
                a => new { a.Name, a.CourseModuleId, a.StartDate, a.EndDate, a.Description },
                activities
                );
            context.SaveChanges();


            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var users = new List<string[]>();
            users.Add(new[] { "teacher@learny.com", "Learny", "Tomas Svensson", RoleName.teacher });
            users.Add(new[] { "student@learny.com", "Learny", "Klas Klättermus", RoleName.student });
            users.Add(new[] { "Abas@learny.com", "Learny", "Abas Ali Rahmanian Aberouie", RoleName.student });
            users.Add(new[] { "Afram@learny.com", "Learny", "Afram Kako", RoleName.student });
            users.Add(new[] { "Archana@learny.com", "Learny", "Archana Chikmagalur Lakshminarasimha", RoleName.student });
            users.Add(new[] { "Christian@learny.com", "Learny", "Christian Trochez Östnaes", RoleName.student });
            users.Add(new[] { "Christopher@learny.com", "Learny", "Christopher Wolf", RoleName.student });
            users.Add(new[] { "Daniel@learny.com", "Learny", "Daniel Saar Odhammer", RoleName.student });
            users.Add(new[] { "Egidio@learny.com", "Learny", "Egidio Palmo Ricchiuti", RoleName.student });
            users.Add(new[] { "Gunnar@learny.com", "Learny", "Gunnar Rydberg", RoleName.student });
            users.Add(new[] { "Hans@learny.com", "Learny", "Hans Guldager", RoleName.student });
            users.Add(new[] { "James@learny.com", "Learny", "James Allen", RoleName.student });
            users.Add(new[] { "John@learny.com", "Learny", "John Alkas Yousef", RoleName.student });
            users.Add(new[] { "Juha@learny.com", "Learny", "Juha Kuusjärvi", RoleName.student });
            users.Add(new[] { "Lars@learny.com", "Learny", "Lars Börlin", RoleName.student });
            users.Add(new[] { "Linus@learny.com", "Learny", "Linus Alpsten", RoleName.student });
            users.Add(new[] { "Nils@learny.com", "Learny", "Nils Lindstedt", RoleName.student });
            users.Add(new[] { "Ola@learny.com", "Learny", "Ola Bjelving", RoleName.student });
            users.Add(new[] { "Petra@learny.com", "Learny", "Petra Lindell", RoleName.student });
            users.Add(new[] { "Rolf@learny.com", "Learny", "Rolf Lundqvist", RoleName.student });
            users.Add(new[] { "Sattar@learny.com", "Learny", "Sattar Alvandpour", RoleName.student });
            users.Add(new[] { "Tensaeberhan@learny.com", "Learny", "Tensaeberhan Mengist Yoseph", RoleName.student });
            users.Add(new[] { "Thomas@learny.com", "Learny", "Thomas Palestig", RoleName.student });
            users.Add(new[] { "Tomas@learny.com", "Learny", "Tomas Lanquist", RoleName.student });
            users.Add(new[] { "Vijayalakshmi@learny.com", "Learny", "Vijayalakshmi Goduguluri", RoleName.student });
            users.Add(new[] { "Zareen@learny.com", "Learny", "Zareen Pathan", RoleName.student });

            foreach (var user in users)
            {
                var userUserName = user[0];
                var userEmail = user[0];
                var userPassword = user[1];
                var userName = user[2];
                var userRole = user[3];

                if (context.Users.Any(u => u.UserName == userUserName)) continue;

                var newUser = new ApplicationUser { UserName = userUserName, Email = userEmail, Name = userName };
                if (userRole == RoleName.student)
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

            #endregion
        }
    }
}
