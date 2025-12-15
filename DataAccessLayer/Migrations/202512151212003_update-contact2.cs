namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecontact2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "IsTrash", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contacts", "IsTrash");
        }
    }
}
