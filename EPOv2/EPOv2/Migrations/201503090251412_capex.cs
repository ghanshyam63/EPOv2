namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class capex : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderDashboardViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(),
                        TempOrderNumber = c.String(),
                        Date = c.String(),
                        Supplier = c.String(),
                        Total = c.Double(nullable: false),
                        Status = c.String(),
                        isEditLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OrderItems", "Capex_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "Capex_Id");
            DropTable("dbo.OrderDashboardViewModels");
        }
    }
}
