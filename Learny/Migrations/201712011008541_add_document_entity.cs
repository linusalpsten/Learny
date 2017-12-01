namespace Learny.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_document_entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false),
                        ContentType = c.String(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        FileName = c.String(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Documents");
        }
    }
}
