namespace FLive.Web.MediaService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaAssets", "FliveWorkoutId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaAssets", "FliveWorkoutId");
        }
    }
}
