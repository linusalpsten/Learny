namespace Learny.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coursecode_is_unique : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "CourseCode", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.Courses", "CourseCode", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Courses", new[] { "CourseCode" });
            AlterColumn("dbo.Courses", "CourseCode", c => c.String(nullable: false));
        }
    }
}
