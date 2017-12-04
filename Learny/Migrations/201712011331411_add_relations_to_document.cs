namespace Learny.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_relations_to_document : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "CourseId", c => c.Int());
            AddColumn("dbo.Documents", "CourseModuleId", c => c.Int());
            AddColumn("dbo.Documents", "ModuleActivityId", c => c.Int());
            AddColumn("dbo.Documents", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Documents", "CourseId");
            CreateIndex("dbo.Documents", "CourseModuleId");
            CreateIndex("dbo.Documents", "ModuleActivityId");
            CreateIndex("dbo.Documents", "UserId");
            AddForeignKey("dbo.Documents", "ModuleActivityId", "dbo.ModuleActivities", "Id");
            AddForeignKey("dbo.Documents", "CourseId", "dbo.Courses", "Id");
            AddForeignKey("dbo.Documents", "CourseModuleId", "dbo.CourseModules", "Id");
            AddForeignKey("dbo.Documents", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "CourseModuleId", "dbo.CourseModules");
            DropForeignKey("dbo.Documents", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Documents", "ModuleActivityId", "dbo.ModuleActivities");
            DropIndex("dbo.Documents", new[] { "UserId" });
            DropIndex("dbo.Documents", new[] { "ModuleActivityId" });
            DropIndex("dbo.Documents", new[] { "CourseModuleId" });
            DropIndex("dbo.Documents", new[] { "CourseId" });
            DropColumn("dbo.Documents", "UserId");
            DropColumn("dbo.Documents", "ModuleActivityId");
            DropColumn("dbo.Documents", "CourseModuleId");
            DropColumn("dbo.Documents", "CourseId");
        }
    }
}
