namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BankMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        BankId = c.Int(nullable: false),
                        AccountType = c.Int(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banks", t => t.BankId, cascadeDelete: true)
                .Index(t => t.BankId);
            
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
           
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankId = c.Int(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Banks", t => t.BankId, cascadeDelete: true)
                .Index(t => t.BankId);

            var b = new BankDbContext()
                    .Banks.Add(
                                new Models.Bank { Name = "Lloyds" }
                                );
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "BankId", "dbo.Banks");
            DropForeignKey("dbo.Accounts", "BankId", "dbo.Banks");
            DropIndex("dbo.Customers", new[] { "BankId" });
            DropIndex("dbo.Accounts", new[] { "BankId" });
            DropTable("dbo.Customers");
            DropTable("dbo.Banks");
            DropTable("dbo.Accounts");
        }
    }
}
