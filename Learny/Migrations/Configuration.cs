namespace Learny.Migrations
{
    using Learny.Models;
    using Learny.Settings;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

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
                    Description="",
                    StartDate = startDate, EndDate = startDate.AddMonths(4) },

                new Course { CourseCode = "113544-1", Name = "Certifierad programerare",
                    Description="Utbildingen leder till at bli Certifierad programmerare. Programmeringsutbildningen gör att deltagare ska kunna upprätta program- och databasstrukturer samt programmera, implementera och underhålla system inom affärssystem, säljstödssystem, statistiksystem, bokningssystem, mm. Den studerande skall även inneha förutsättningarna för att kunna certifiera sig till MCSD: Microsoft Certified Solutions Developer.",
                    StartDate = new DateTime(2017, 07, 31), EndDate = new DateTime(2017, 09, 29) },

                new Course { CourseCode = "DNV17", Name = "Systemutvecklare.NET",
                    Description="",
                    StartDate = new DateTime(2017, 12, 18), EndDate = new DateTime(2018, 03, 14) },

                new Course { CourseCode = "DNVT18", Name = "Systemutvecklare.NET",
                    Description="",
                    StartDate = new DateTime(2018, 02, 05), EndDate = new DateTime(2018, 05, 02) },

                new Course { CourseCode = "JHT17", Name = "Systemutvecklare Java",
                    Description="",
                    StartDate = new DateTime(2017, 09, 04), EndDate = new DateTime(2018, 01, 12) },

                new Course { CourseCode = "113544-2", Name = "Certifierad programerare",
                    Description="",
                    StartDate = new DateTime(2017, 08, 28), EndDate = new DateTime(2017, 10, 27) },

                new Course { CourseCode = "109522-1", Name = "IT-tekniker SharePoint/Dynamics",
                    Description="",
                    StartDate = new DateTime(2017, 12, 18), EndDate = new DateTime(2018, 03, 14) },

                new Course { CourseCode = "109522-2", Name = "IT-tekniker SharePoint/Dynamics",
                    Description="",
                    StartDate = new DateTime(2018, 01, 22), EndDate = new DateTime(2018, 04, 17) },
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
                new CourseModule { Name = "App.Utv.", StartDate = startDate.AddDays(74),
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

            var cSharpName = modules[0].Name;
            var cSharpDate = modules[0].StartDate;
            var cSharpId = context.Modules.Where(m => m.Name == cSharpName && m.StartDate == cSharpDate).FirstOrDefault().Id;

            var testName = modules[1].Name;
            var testDate = modules[1].StartDate;
            var testId = context.Modules.Where(m => m.Name == testName && m.StartDate == testDate).FirstOrDefault().Id;

            var webbName = modules[2].Name;
            var webbDate = modules[2].StartDate;
            var webbId = context.Modules.Where(m => m.Name == webbName && m.StartDate == webbDate).FirstOrDefault().Id;

            var mvcName = modules[3].Name;
            var mvcDate = modules[3].StartDate;
            var mvcId = context.Modules.Where(m => m.Name == mvcName && m.StartDate == mvcDate).FirstOrDefault().Id;

            var dbName = modules[4].Name;
            var dbDate = modules[4].StartDate;
            var dbId = context.Modules.Where(m => m.Name == dbName && m.StartDate == dbDate).FirstOrDefault().Id;

            var appName = modules[5].Name;
            var appDate = modules[5].StartDate;
            var appId = context.Modules.Where(m => m.Name == appName && m.StartDate == appDate).FirstOrDefault().Id;

            var mvcAdvName = modules[6].Name;
            var mvcAdvDate = modules[6].StartDate;
            var mvcAdvId = context.Modules.Where(m => m.Name == mvcAdvName && m.StartDate == mvcAdvDate).FirstOrDefault().Id;

            var activities = new ModuleActivity[]
            {
                new ModuleActivity {
                    Name = "Intro", Description ="",
                    StartDate = startDate.AddDays(0), EndDate = startDate.AddDays(0),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 1.1, 1.2", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(0), EndDate = startDate.AddDays(0),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 1.3", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(1), EndDate = startDate.AddDays(1),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 1.4 + 1.5", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(1), EndDate = startDate.AddDays(1),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "C# Intro", Description ="Adrian",
                    StartDate = startDate.AddDays(2), EndDate = startDate.AddDays(2),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Övning 2", Description ="Flow Control",
                    StartDate = startDate.AddDays(3), EndDate = startDate.AddDays(3),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "C# Grund", Description ="Adrian",
                    StartDate = startDate.AddDays(4), EndDate = startDate.AddDays(4),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 1.6 + 1.7", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(7), EndDate = startDate.AddDays(7),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 1.8", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(7), EndDate = startDate.AddDays(7),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 1.7 + 1.8", Description ="C# Fundamentals with Visual Studio 2015 med Scott Allen",
                    StartDate = startDate.AddDays(8), EndDate = startDate.AddDays(8),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Övning 2", Description ="Flow Control",
                    StartDate = startDate.AddDays(8), EndDate = startDate.AddDays(8),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Repition", Description ="",
                    StartDate = startDate.AddDays(8), EndDate = startDate.AddDays(8),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "OOP", Description ="Adrian",
                    StartDate = startDate.AddDays(9), EndDate = startDate.AddDays(9),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Övning 3", Description =" Inkapsling, arv och polymorfism Nytt utkast",
                    StartDate = startDate.AddDays(10), EndDate = startDate.AddDays(10),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "OOP 2", Description ="Adrian",
                    StartDate = startDate.AddDays(11), EndDate = startDate.AddDays(11),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Övning 3", Description =" Inkapsling, arv och polymorfism Nytt utkast",
                    StartDate = startDate.AddDays(14), EndDate = startDate.AddDays(14),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Kapitel 2.1 - 2.4", Description ="",
                    StartDate = startDate.AddDays(15), EndDate = startDate.AddDays(15),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Övning 4", Description ="",
                    StartDate = startDate.AddDays(15), EndDate = startDate.AddDays(15),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Generics", Description ="Adrian",
                    StartDate = startDate.AddDays(21), EndDate = startDate.AddDays(21),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "LINQ", Description ="Adrian",
                    StartDate = startDate.AddDays(21), EndDate = startDate.AddDays(21),
                    ActivityTypeId = lectureTypeId,
                    CourseModuleId = cSharpId },

                new ModuleActivity {
                    Name = "Unit Test", Description ="C# Best Practices: Improving on the Basics med Deborah Kurata",
                    StartDate = startDate.AddDays(22), EndDate = startDate.AddDays(22),
                    ActivityTypeId = elearningTypeId,
                    CourseModuleId = testId },

                new ModuleActivity {
                    Name = "Sprint review 3", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(107), EndDate = startDate.AddDays(107),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvId },

                new ModuleActivity {
                    Name = "Projekt slutfas", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(108), EndDate = startDate.AddDays(108),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvId },

                new ModuleActivity {
                    Name = "Slutredovisning", Description ="Slutprojekt",
                    StartDate = startDate.AddDays(109), EndDate = startDate.AddDays(109),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvId },

                new ModuleActivity {
                    Name = "Avslutning", Description ="",
                    StartDate = startDate.AddDays(109), EndDate = startDate.AddDays(109),
                    ActivityTypeId = exerciseTypeId,
                    CourseModuleId = mvcAdvId },


                new ModuleActivity {Name = "Generics", Description ="",StartDate = startDate.AddDays(16), EndDate = startDate.AddDays(16),ActivityTypeId = lectureTypeId, CourseModuleId = cSharpId },
                new ModuleActivity {Name = "Kapitel 2.5 – 2.6", Description ="",StartDate = startDate.AddDays(17), EndDate = startDate.AddDays(17),ActivityTypeId = elearningTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Övning 4", Description ="",StartDate = startDate.AddDays(17), EndDate = startDate.AddDays(17),ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Kapitel 2.7 – 2.9", Description ="",StartDate = startDate.AddDays(18), EndDate = startDate.AddDays(18),ActivityTypeId = elearningTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Övning 4", Description ="",StartDate = startDate.AddDays(18), EndDate = startDate.AddDays(18),ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Ariktektur", Description ="",StartDate = startDate.AddDays(23), EndDate = startDate.AddDays(23),ActivityTypeId = lectureTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Test", Description ="",StartDate = startDate.AddDays(23), EndDate = startDate.AddDays(23),ActivityTypeId = lectureTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Övning Garage 1.0", Description ="",StartDate = startDate.AddDays(24), EndDate = startDate.AddDays(25),ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Övning Garage 1.0", Description ="",StartDate = startDate.AddDays(28), EndDate = startDate.AddDays(29),ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Redovisning", Description ="",StartDate = startDate.AddDays(29), EndDate = startDate.AddDays(29),ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "E-L: 3 + 4.1-4.3", Description ="",StartDate = startDate.AddDays(30), EndDate = startDate.AddDays(30),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Övning HTML", Description ="",StartDate = startDate.AddDays(30), EndDate = startDate.AddDays(30),ActivityTypeId = exerciseTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "E-L: 4.4-4.5", Description ="",StartDate = startDate.AddDays(31), EndDate = startDate.AddDays(31),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Övning CSS", Description ="",StartDate = startDate.AddDays(31), EndDate = startDate.AddDays(31),ActivityTypeId = exerciseTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Html/Css", Description ="",StartDate = startDate.AddDays(32), EndDate = startDate.AddDays(32),ActivityTypeId = lectureTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "JavaScript CodeSchool", Description ="",StartDate = startDate.AddDays(35), EndDate = startDate.AddDays(35),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "E-L: 5.1 – 5.3", Description ="",StartDate = startDate.AddDays(36), EndDate = startDate.AddDays(36),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "E-L: 5.3 – 5.5", Description ="",StartDate = startDate.AddDays(36), EndDate = startDate.AddDays(36),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "JS", Description ="",StartDate = startDate.AddDays(37), EndDate = startDate.AddDays(37),ActivityTypeId = lectureTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Övning JS", Description ="",StartDate = startDate.AddDays(37), EndDate = startDate.AddDays(37),ActivityTypeId = lectureTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Övning JS", Description ="",StartDate = startDate.AddDays(38), EndDate = startDate.AddDays(38),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Kapitel 6 Bootstrap", Description ="",StartDate = startDate.AddDays(39), EndDate = startDate.AddDays(39),ActivityTypeId = elearningTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "Övning Bootstrap", Description ="",StartDate = startDate.AddDays(42), EndDate = startDate.AddDays(43),ActivityTypeId = exerciseTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "ASP.NET MVC", Description ="",StartDate = startDate.AddDays(44), EndDate = startDate.AddDays(44),ActivityTypeId = lectureTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Kapitel 7.1 – 7.3", Description ="",StartDate = startDate.AddDays(45), EndDate = startDate.AddDays(45),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "MVC", Description ="",StartDate = startDate.AddDays(46), EndDate = startDate.AddDays(46),ActivityTypeId = lectureTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Kapitel 7.4 ", Description ="",StartDate = startDate.AddDays(49), EndDate = startDate.AddDays(49),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Kapitel 7.5 + Övn MVC ", Description ="",StartDate = startDate.AddDays(49), EndDate = startDate.AddDays(49),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "E-L Repetition + Övn ", Description ="",StartDate = startDate.AddDays(50), EndDate = startDate.AddDays(50),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Kapitel 7.6 + Övn ", Description ="",StartDate = startDate.AddDays(50), EndDate = startDate.AddDays(50),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Git ", Description ="",StartDate = startDate.AddDays(51), EndDate = startDate.AddDays(51),ActivityTypeId = lectureTypeId, CourseModuleId = webbId },
new ModuleActivity {Name = "E-L Repetition + Övn ", Description ="",StartDate = startDate.AddDays(52), EndDate = startDate.AddDays(52),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "7.6 + Övn ", Description ="",StartDate = startDate.AddDays(52), EndDate = startDate.AddDays(52),ActivityTypeId = elearningTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "ViewModel ", Description ="",StartDate = startDate.AddDays(53), EndDate = startDate.AddDays(53),ActivityTypeId = lectureTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "PartialView ", Description ="",StartDate = startDate.AddDays(53), EndDate = startDate.AddDays(53),ActivityTypeId = lectureTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Övning Garage 2.0 ", Description ="",StartDate = startDate.AddDays(56), EndDate = startDate.AddDays(59),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Redovisning ", Description ="",StartDate = startDate.AddDays(59), EndDate = startDate.AddDays(59),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcId },
new ModuleActivity {Name = "Datamodellering ", Description ="",StartDate = startDate.AddDays(60), EndDate = startDate.AddDays(60),ActivityTypeId = lectureTypeId, CourseModuleId = dbId },
new ModuleActivity {Name = "Övning 13 ", Description ="",StartDate = startDate.AddDays(60), EndDate = startDate.AddDays(60),ActivityTypeId = exerciseTypeId, CourseModuleId = dbId },
new ModuleActivity {Name = "EntityFramework ", Description ="",StartDate = startDate.AddDays(63), EndDate = startDate.AddDays(63),ActivityTypeId = lectureTypeId, CourseModuleId = dbId },
new ModuleActivity {Name = "SQLBolt.com ", Description ="",StartDate = startDate.AddDays(64), EndDate = startDate.AddDays(64),ActivityTypeId = elearningTypeId, CourseModuleId = dbId },
new ModuleActivity {Name = "Test MVC ", Description ="",StartDate = startDate.AddDays(65), EndDate = startDate.AddDays(65),ActivityTypeId = lectureTypeId, CourseModuleId = testId },
new ModuleActivity {Name = "Continuous intergration", Description ="",StartDate = startDate.AddDays(65), EndDate = startDate.AddDays(65),ActivityTypeId = lectureTypeId, CourseModuleId = testId },
new ModuleActivity {Name = "Garage 2.5 ", Description ="",StartDate = startDate.AddDays(66), EndDate = startDate.AddDays(67),ActivityTypeId = exerciseTypeId, CourseModuleId = dbId },
new ModuleActivity {Name = "Garage 2.5 ", Description ="",StartDate = startDate.AddDays(70), EndDate = startDate.AddDays(71),ActivityTypeId = exerciseTypeId, CourseModuleId = cSharpId },
new ModuleActivity {Name = "Kapitel 10", Description ="",StartDate = startDate.AddDays(71), EndDate = startDate.AddDays(71),ActivityTypeId = elearningTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "Övn UX ", Description ="",StartDate = startDate.AddDays(71), EndDate = startDate.AddDays(71),ActivityTypeId = exerciseTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "UX ", Description ="",StartDate = startDate.AddDays(72), EndDate = startDate.AddDays(72),ActivityTypeId = lectureTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "Övning 16 ", Description ="",StartDate = startDate.AddDays(72), EndDate = startDate.AddDays(72),ActivityTypeId = exerciseTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "Jquery CodeSchool ", Description ="",StartDate = startDate.AddDays(73), EndDate = startDate.AddDays(73),ActivityTypeId = elearningTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "Jquery/Ajax ", Description ="",StartDate = startDate.AddDays(74), EndDate = startDate.AddDays(74),ActivityTypeId = lectureTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "Identity ", Description ="",StartDate = startDate.AddDays(77), EndDate = startDate.AddDays(77),ActivityTypeId = lectureTypeId, CourseModuleId = appId },
new ModuleActivity {Name = "Kapitel 12 MVC ", Description ="",StartDate = startDate.AddDays(78), EndDate = startDate.AddDays(78),ActivityTypeId = elearningTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Övning 17 ", Description ="",StartDate = startDate.AddDays(78), EndDate = startDate.AddDays(78),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Client vs. Server ", Description ="",StartDate = startDate.AddDays(79), EndDate = startDate.AddDays(79),ActivityTypeId = lectureTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Kapitel 12 MVC ", Description ="",StartDate = startDate.AddDays(80), EndDate = startDate.AddDays(80),ActivityTypeId = elearningTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Övning 17 ", Description ="",StartDate = startDate.AddDays(80), EndDate = startDate.AddDays(80),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "SCRUM ", Description ="",StartDate = startDate.AddDays(81), EndDate = startDate.AddDays(81),ActivityTypeId = lectureTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projektplanering ", Description ="",StartDate = startDate.AddDays(84), EndDate = startDate.AddDays(85),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projektstart ", Description ="",StartDate = startDate.AddDays(86), EndDate = startDate.AddDays(86),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Planering sprint 1 ", Description ="",StartDate = startDate.AddDays(86), EndDate = startDate.AddDays(86),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projekt sprint 1", Description ="",StartDate = startDate.AddDays(87), EndDate = startDate.AddDays(88),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projekt sprint 1", Description ="",StartDate = startDate.AddDays(91), EndDate = startDate.AddDays(92),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Sprint review 1", Description ="",StartDate = startDate.AddDays(93), EndDate = startDate.AddDays(93),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Planering sprint 2 ", Description ="",StartDate = startDate.AddDays(93), EndDate = startDate.AddDays(93),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projekt sprint 2", Description ="",StartDate = startDate.AddDays(94), EndDate = startDate.AddDays(95),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projekt sprint 2", Description ="",StartDate = startDate.AddDays(98), EndDate = startDate.AddDays(99),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Sprint review 2", Description ="",StartDate = startDate.AddDays(100), EndDate = startDate.AddDays(100),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Planering sprint 3", Description ="",StartDate = startDate.AddDays(100), EndDate = startDate.AddDays(100),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projekt sprint 3", Description ="",StartDate = startDate.AddDays(101), EndDate = startDate.AddDays(102),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },
new ModuleActivity {Name = "Projekt sprint 3", Description ="",StartDate = startDate.AddDays(105), EndDate = startDate.AddDays(106),ActivityTypeId = exerciseTypeId, CourseModuleId = mvcAdvId },



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
