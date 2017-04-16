namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recognition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recognitions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(nullable: false),
                        BadgeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Badges", t => t.BadgeId, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.BadgeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recognitions", "PersonId", "dbo.People");
            DropForeignKey("dbo.Recognitions", "BadgeId", "dbo.Badges");
            DropIndex("dbo.Recognitions", new[] { "BadgeId" });
            DropIndex("dbo.Recognitions", new[] { "PersonId" });
            DropTable("dbo.Recognitions");
        }
    }
}
