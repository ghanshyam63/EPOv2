namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DTiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Site = c.Int(nullable: false),
                        Department = c.Int(nullable: false),
                        TileType = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        DUserGroup_Id = c.Int(),
                        DUserGroup_Id1 = c.Int(),
                        UserDashboardSettings_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DUserGroups", t => t.DUserGroup_Id)
                .ForeignKey("dbo.DUserGroups", t => t.DUserGroup_Id1)
                .ForeignKey("dbo.UserDashboardSettings", t => t.UserDashboardSettings_Id)
                .Index(t => t.DUserGroup_Id)
                .Index(t => t.DUserGroup_Id1)
                .Index(t => t.UserDashboardSettings_Id);
            
            CreateTable(
                "dbo.DUserGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserDashboardSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        DUserGroup_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DUserGroups", t => t.DUserGroup_Id)
                .Index(t => t.DUserGroup_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DTiles", "UserDashboardSettings_Id", "dbo.UserDashboardSettings");
            DropForeignKey("dbo.UserDashboardSettings", "DUserGroup_Id", "dbo.DUserGroups");
            DropForeignKey("dbo.DTiles", "DUserGroup_Id1", "dbo.DUserGroups");
            DropForeignKey("dbo.DTiles", "DUserGroup_Id", "dbo.DUserGroups");
            DropIndex("dbo.UserDashboardSettings", new[] { "DUserGroup_Id" });
            DropIndex("dbo.DTiles", new[] { "UserDashboardSettings_Id" });
            DropIndex("dbo.DTiles", new[] { "DUserGroup_Id1" });
            DropIndex("dbo.DTiles", new[] { "DUserGroup_Id" });
            DropTable("dbo.UserDashboardSettings");
            DropTable("dbo.DUserGroups");
            DropTable("dbo.DTiles");
        }
    }
}
