namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DUserGroups", "DefaultTiles_SerializedValue", c => c.String());
            AddColumn("dbo.UserDashboardSettings", "MyTiles_SerializedValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDashboardSettings", "MyTiles_SerializedValue");
            DropColumn("dbo.DUserGroups", "DefaultTiles_SerializedValue");
        }
    }
}
