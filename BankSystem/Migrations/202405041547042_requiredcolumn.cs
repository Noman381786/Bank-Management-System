namespace BankSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredcolumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TransactionHistories", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.TransactionHistories", "TransactionType", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TransactionHistories", "TransactionType", c => c.String());
            AlterColumn("dbo.TransactionHistories", "Description", c => c.String());
        }
    }
}
