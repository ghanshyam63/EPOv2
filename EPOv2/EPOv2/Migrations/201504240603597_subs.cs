namespace EPOv2.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class subs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubstituteApprovers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false, maxLength: 200),
                        LastModifiedBy = c.String(nullable: false, maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ApproverUser_Id = c.String(maxLength: 128),
                        SubstitutionUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ApproverUser_Id)
                .ForeignKey("dbo.Users", t => t.SubstitutionUser_Id)
                .Index(t => t.ApproverUser_Id)
                .Index(t => t.SubstitutionUser_Id);
            
            DropTable("dbo.OrderDashboardViewModels");
        }
        
        public override void Down()
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
            
            DropForeignKey("dbo.SubstituteApprovers", "SubstitutionUser_Id", "dbo.Users");
            DropForeignKey("dbo.SubstituteApprovers", "ApproverUser_Id", "dbo.Users");
            DropIndex("dbo.SubstituteApprovers", new[] { "SubstitutionUser_Id" });
            DropIndex("dbo.SubstituteApprovers", new[] { "ApproverUser_Id" });
            DropTable("dbo.SubstituteApprovers");
        }
    }
}
