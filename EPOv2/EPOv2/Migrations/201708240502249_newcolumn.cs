namespace EPOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newcolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Approvers", "OldApprover", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Approvers", "OldApprover");
        }
    }
}
