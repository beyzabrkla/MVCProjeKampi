namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemessage2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "IsTrash", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "IsTrash");
        }
    }
}
