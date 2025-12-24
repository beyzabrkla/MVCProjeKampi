namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SourceFolder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "SourceFolder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "SourceFolder");
        }
    }
}
