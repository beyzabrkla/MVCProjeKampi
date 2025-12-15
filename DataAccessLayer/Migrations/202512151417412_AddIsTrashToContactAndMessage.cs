namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsTrashToContactAndMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "TrashDate", c => c.DateTime());
            AddColumn("dbo.Messages", "TrashDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "TrashDate");
            DropColumn("dbo.Contacts", "TrashDate");
        }
    }
}
