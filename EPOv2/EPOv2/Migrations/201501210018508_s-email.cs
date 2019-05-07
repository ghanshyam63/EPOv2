namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class semail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "SupplierEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "SupplierEmail");
        }
    }
}
