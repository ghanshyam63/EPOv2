namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DUserGroups", "DefaultTiles", c => c.String());
            AddColumn("dbo.DUserGroups", "RequiredTiles", c => c.String());
            AddColumn("dbo.UserDashboardSettings", "MyTiles", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDashboardSettings", "MyTiles_SerializedValue");
            DropColumn("dbo.DUserGroups", "DefaultTiles_SerializedValue");
            DropColumn("dbo.DUserGroups", "RequiredTiles_SerializedValue");
        }
    }
}
