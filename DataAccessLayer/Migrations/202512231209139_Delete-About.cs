namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteAbout : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Abouts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Abouts",
                c => new
                    {
                        AboutId = c.Int(nullable: false, identity: true),
                        AboutDetails1 = c.String(maxLength: 1000),
                        AboutDetails2 = c.String(maxLength: 1000),
                        AboutImage1 = c.String(maxLength: 100),
                        AboutImage2 = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AboutId);
            
        }
    }
}
