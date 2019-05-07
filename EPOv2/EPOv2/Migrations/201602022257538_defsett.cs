namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class defsett : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.UserOrderSettings", name: "DefualtDeliveryAddress_Id", newName: "DefaultDeliveryAddress_Id");
            RenameIndex(table: "dbo.UserOrderSettings", name: "IX_DefualtDeliveryAddress_Id", newName: "IX_DefaultDeliveryAddress_Id");
        }
        
        public override void Down()
        {
           
            RenameIndex(table: "dbo.UserOrderSettings", name: "IX_DefaultDeliveryAddress_Id", newName: "IX_DefualtDeliveryAddress_Id");
            RenameColumn(table: "dbo.UserOrderSettings", name: "DefaultDeliveryAddress_Id", newName: "DefualtDeliveryAddress_Id");
        }
    }
}
