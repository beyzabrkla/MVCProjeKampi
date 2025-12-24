namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSenderAndReceiverTrash : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "SenderTrash", c => c.Boolean(nullable: false));
            AddColumn("dbo.Messages", "ReceiverTrash", c => c.Boolean(nullable: false));
            DropColumn("dbo.Messages", "IsTrash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "IsTrash", c => c.Boolean(nullable: false));
            DropColumn("dbo.Messages", "ReceiverTrash");
            DropColumn("dbo.Messages", "SenderTrash");
        }
    }
}
