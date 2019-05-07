namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newcolumnoldapproverlist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CapexApprovers", "oldapprover", c => c.String());
            AddColumn("dbo.VoucherDocuments", "oldAuthoriser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoucherDocuments", "oldAuthoriser");
            DropColumn("dbo.CapexApprovers", "oldapprover");
        }
    }
}
