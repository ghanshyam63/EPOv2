namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class calEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DCalendarEvents", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DCalendarEvents", "Title");
        }
    }
}
