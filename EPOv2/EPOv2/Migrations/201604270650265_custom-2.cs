namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DTiles", "DUserGroup_Id", "dbo.DUserGroups");
            DropForeignKey("dbo.DTiles", "DUserGroup_Id1", "dbo.DUserGroups");
            DropForeignKey("dbo.DTiles", "UserDashboardSettings_Id", "dbo.UserDashboardSettings");
            DropIndex("dbo.DTiles", new[] { "DUserGroup_Id" });
            DropIndex("dbo.DTiles", new[] { "DUserGroup_Id1" });
            DropIndex("dbo.DTiles", new[] { "UserDashboardSettings_Id" });
            AddColumn("dbo.DUserGroups", "RequiredTiles_SerializedValue", c => c.String());
            DropColumn("dbo.DTiles", "DUserGroup_Id");
            DropColumn("dbo.DTiles", "DUserGroup_Id1");
            DropColumn("dbo.DTiles", "UserDashboardSettings_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DTiles", "UserDashboardSettings_Id", c => c.Int());
            AddColumn("dbo.DTiles", "DUserGroup_Id1", c => c.Int());
            AddColumn("dbo.DTiles", "DUserGroup_Id", c => c.Int());
            DropColumn("dbo.DUserGroups", "RequiredTiles_SerializedValue");
            CreateIndex("dbo.DTiles", "UserDashboardSettings_Id");
            CreateIndex("dbo.DTiles", "DUserGroup_Id1");
            CreateIndex("dbo.DTiles", "DUserGroup_Id");
            AddForeignKey("dbo.DTiles", "UserDashboardSettings_Id", "dbo.UserDashboardSettings", "Id");
            AddForeignKey("dbo.DTiles", "DUserGroup_Id1", "dbo.DUserGroups", "Id");
            AddForeignKey("dbo.DTiles", "DUserGroup_Id", "dbo.DUserGroups", "Id");
        }
    }
}
