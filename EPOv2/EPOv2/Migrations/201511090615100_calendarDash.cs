namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class calendarDash : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DCalendarEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(),
                        IsOneDayEvent = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EventType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DCalendarEventTypes", t => t.EventType_Id)
                .Index(t => t.EventType_Id);
            
            CreateTable(
                "dbo.DCalendarEventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Icon = c.String(),
                        IconColor = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DCalendarEvents", "EventType_Id", "dbo.DCalendarEventTypes");
            DropIndex("dbo.DCalendarEvents", new[] { "EventType_Id" });
            DropTable("dbo.DCalendarEventTypes");
            DropTable("dbo.DCalendarEvents");
        }
    }
}
