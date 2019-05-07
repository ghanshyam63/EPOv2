namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class custom51 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserDashboardSettings_Id", c => c.Int());
            CreateIndex("dbo.Users", "UserDashboardSettings_Id");
            AddForeignKey("dbo.Users", "UserDashboardSettings_Id", "dbo.UserDashboardSettings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserDashboardSettings_Id", "dbo.UserDashboardSettings");
            DropIndex("dbo.Users", new[] { "UserDashboardSettings_Id" });
            DropColumn("dbo.Users", "UserDashboardSettings_Id");
        }
    }
}
