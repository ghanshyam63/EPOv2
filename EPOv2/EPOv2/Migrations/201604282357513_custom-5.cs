namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserDashboardSettings", "MyTiles");
            DropColumn("dbo.DUserGroups", "DefaultTiles");
            DropColumn("dbo.DUserGroups", "RequiredTiles");
            RenameColumn(table: "dbo.DUserGroups", name: "RequiredTiles_SerializedValue", newName: "RequiredTiles");
            RenameColumn(table: "dbo.DUserGroups", name: "DefaultTiles_SerializedValue", newName: "DefaultTiles");
            RenameColumn(table: "dbo.UserDashboardSettings", name: "MyTiles_SerializedValue", newName: "MyTiles");
           
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDashboardSettings", "MyTiles");
            DropColumn("dbo.DUserGroups", "DefaultTiles");
            DropColumn("dbo.DUserGroups", "RequiredTiles");
            RenameColumn(table: "dbo.UserDashboardSettings", name: "MyTiles", newName: "MyTiles_SerializedValue");
            RenameColumn(table: "dbo.DUserGroups", name: "DefaultTiles", newName: "DefaultTiles_SerializedValue");
            RenameColumn(table: "dbo.DUserGroups", name: "RequiredTiles", newName: "RequiredTiles_SerializedValue");
        }
    }
}
