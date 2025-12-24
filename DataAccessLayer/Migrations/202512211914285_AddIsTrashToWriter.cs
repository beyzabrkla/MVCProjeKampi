namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsTrashToWriter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Writers", "IsTrash", c => c.Boolean(nullable: false));
            DropColumn("dbo.Titles", "IsTrash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Titles", "IsTrash", c => c.Boolean(nullable: false));
            DropColumn("dbo.Writers", "IsTrash");
        }
    }
}
