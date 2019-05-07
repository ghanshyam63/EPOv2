namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cal2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DCalendarEvents", "End", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DCalendarEvents", "End", c => c.DateTime());
        }
    }
}
