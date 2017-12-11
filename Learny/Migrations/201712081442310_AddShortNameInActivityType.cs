namespace Learny.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShortNameInActivityType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityTypes", "ShortName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityTypes", "ShortName");
        }
    }
}
