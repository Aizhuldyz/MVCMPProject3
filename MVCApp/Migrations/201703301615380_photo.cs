namespace MVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class photo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "PhotoUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "PhotoUrl");
        }
    }
}
