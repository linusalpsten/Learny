using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Learny.Models
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseModule> Modules { get; set; }
        public DbSet<ModuleActivity> Activities { get; set; }

        public ApplicationDbContext()
            : base("LearnyConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
}