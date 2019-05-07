namespace DomainModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnIsGSTFree : DbMigration
    {
        public override void Up()
        {

            AddColumn("dbo.OrderItems", "IsGSTFree", c => c.Boolean());
            AddColumn("dbo.OrderItemLogs", "IsGStFree", c => c.Boolean());



        }

        public override void Down()
        {
            DropColumn("dbo.OrderItems", "IsGSTFree");
            DropColumn("dbo.OrderItemLogs", "IsGStFree");
        }
    }
}
