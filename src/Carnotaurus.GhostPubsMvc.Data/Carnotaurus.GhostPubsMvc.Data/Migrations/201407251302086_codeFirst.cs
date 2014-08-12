using System.Data.Entity.Migrations;

namespace Carnotaurus.GhostPubsMvc.Data.Migrations
{
    public partial class codeFirst : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Category.AddressType",
                c => new
                {
                    AddressTypeID = c.Int(false, true),
                    LastModified = c.DateTime(false),
                    Name = c.String(false, 50),
                    Description = c.String(maxLength: 300),
                    Deleted = c.DateTime(),
                })
                .PrimaryKey(t => t.AddressTypeID);

            CreateTable(
                "Organisation.Org",
                c => new
                {
                    OrgID = c.Int(false, true),
                    Created = c.DateTime(false),
                    Modified = c.DateTime(false),
                    Deleted = c.DateTime(),
                    AddressTypeID = c.Int(),
                    CountyID = c.Int(),
                    ParentID = c.Int(),
                    TradingStatus = c.Int(),
                    HauntedStatus = c.Int(),
                    TradingName = c.String(false, 300),
                    AlternateName = c.String(maxLength: 300),
                    SearchName = c.String(maxLength: 300),
                    Locality = c.String(maxLength: 300),
                    Town = c.String(maxLength: 300),
                    Administrative_area_level_2 = c.String(maxLength: 300),
                    Postcode = c.String(maxLength: 8),
                    Address = c.String(maxLength: 300),
                    Phone = c.String(maxLength: 50),
                    Twitter = c.String(maxLength: 300),
                    Email = c.String(maxLength: 300),
                    Facebook = c.String(maxLength: 300),
                    Website = c.String(maxLength: 300),
                    OSX = c.Int(),
                    OSY = c.Int(),
                    Lat = c.Double(),
                    Lon = c.Double(),
                    Tried = c.Int(),
                    GoogleMapData = c.String(),
                    ManualConfirmDate = c.DateTime(),
                })
                .PrimaryKey(t => t.OrgID)
                .ForeignKey("Category.AddressType", t => t.AddressTypeID)
                .ForeignKey("dbo.County", t => t.CountyID)
                .Index(t => t.AddressTypeID)
                .Index(t => t.CountyID);

            CreateTable(
                "dbo.BookItem",
                c => new
                {
                    BookItemID = c.Int(false, true),
                    Created = c.DateTime(false),
                    Modified = c.DateTime(false),
                    Deleted = c.DateTime(),
                    BookID = c.Int(),
                    OrgID = c.Int(),
                    County = c.String(),
                    Town = c.String(),
                    AlternativeTown = c.String(),
                    TradingName = c.String(),
                    Text = c.String(),
                    Postcode = c.String(maxLength: 50),
                })
                .PrimaryKey(t => t.BookItemID)
                .ForeignKey("dbo.Book", t => t.BookID)
                .ForeignKey("Organisation.Org", t => t.OrgID)
                .Index(t => t.BookID)
                .Index(t => t.OrgID);

            CreateTable(
                "dbo.Book",
                c => new
                {
                    BookID = c.Int(false, true),
                    Created = c.DateTime(),
                    Modified = c.DateTime(),
                    Deleted = c.DateTime(),
                    Name = c.String(false),
                })
                .PrimaryKey(t => t.BookID);

            CreateTable(
                "dbo.County",
                c => new
                {
                    CountyID = c.Int(false, true),
                    RegionID = c.Int(false),
                    Name = c.String(maxLength: 300),
                    Description = c.String(false),
                })
                .PrimaryKey(t => t.CountyID)
                .ForeignKey("dbo.Region", t => t.RegionID, true)
                .Index(t => t.RegionID);

            CreateTable(
                "dbo.Region",
                c => new
                {
                    RegionID = c.Int(false, true),
                    Name = c.String(maxLength: 100),
                    Description = c.String(),
                    Deleted = c.DateTime(),
                })
                .PrimaryKey(t => t.RegionID);

            CreateTable(
                "Organisation.Note",
                c => new
                {
                    NoteID = c.Int(false, true),
                    LastModified = c.DateTime(false),
                    CreateDate = c.DateTime(false),
                    OrgID = c.Int(false),
                    Text = c.String(false),
                    Source = c.String(),
                })
                .PrimaryKey(t => t.NoteID)
                .ForeignKey("Organisation.Org", t => t.OrgID, true)
                .Index(t => t.OrgID);

            CreateTable(
                "Organisation.Tag",
                c => new
                {
                    TagID = c.Int(false, true),
                    LastModified = c.DateTime(false),
                    OrgID = c.Int(),
                    FeatureID = c.Int(false),
                })
                .PrimaryKey(t => t.TagID)
                .ForeignKey("Organisation.Feature", t => t.FeatureID, true)
                .ForeignKey("Organisation.Org", t => t.OrgID)
                .Index(t => t.OrgID)
                .Index(t => t.FeatureID);

            CreateTable(
                "Organisation.Feature",
                c => new
                {
                    FeatureID = c.Int(false, true),
                    LastModified = c.DateTime(false),
                    FeatureTypeID = c.Int(false),
                    Name = c.String(maxLength: 300),
                })
                .PrimaryKey(t => t.FeatureID)
                .ForeignKey("Category.FeatureType", t => t.FeatureTypeID, true)
                .Index(t => t.FeatureTypeID);

            CreateTable(
                "Category.FeatureType",
                c => new
                {
                    FeatureTypeID = c.Int(false, true),
                    ParentFeatureTypeID = c.Int(),
                    LastModified = c.DateTime(false),
                    Name = c.String(false, 50),
                    Description = c.String(maxLength: 300),
                    Deleted = c.DateTime(),
                })
                .PrimaryKey(t => t.FeatureTypeID);

            CreateTable(
                "dbo.Category",
                c => new
                {
                    CategoryID = c.Int(false, true),
                    Name = c.String(false, 250),
                })
                .PrimaryKey(t => t.CategoryID);

            CreateTable(
                "dbo.ContentPage",
                c => new
                {
                    PageID = c.Int(false, true),
                    CategoryID = c.Int(),
                    OrderID = c.Int(),
                    PageTemplateID = c.Int(),
                    Name = c.String(false, 50),
                    Title = c.String(maxLength: 150),
                    Lead = c.String(maxLength: 1000),
                    WellHeader = c.String(maxLength: 1000),
                    Description = c.String(maxLength: 1000),
                    HtmlText = c.String(),
                    DateCreated = c.DateTime(),
                    DateModified = c.DateTime(),
                    Deleted = c.DateTime(),
                })
                .PrimaryKey(t => t.PageID)
                .ForeignKey("dbo.Category", t => t.CategoryID)
                .Index(t => t.CategoryID);

            CreateTable(
                "oe.LocalAuthorities",
                c => new
                {
                    Id = c.Int(false, true),
                    LocalAuthority = c.String(name: "Local Authority", maxLength: 255),
                    Region = c.String(maxLength: 255),
                    Numberofbreweries = c.Double(name: "Number of breweries"),
                    Numberofpubs = c.Double(name: "Number of pubs"),
                    Employment = c.Double(),
                    F6 = c.Double(),
                    F7 = c.Double(),
                    F8 = c.Double(),
                    Wagesm = c.Decimal(name: "Wages (£m)", precision: 18, scale: 2),
                    F10 = c.Decimal(precision: 18, scale: 2),
                    F11 = c.Decimal(precision: 18, scale: 2),
                    F12 = c.Decimal(precision: 18, scale: 2),
                    GVAm = c.Decimal(name: "GVA (£m)", precision: 18, scale: 2),
                    F14 = c.Decimal(precision: 18, scale: 2),
                    F15 = c.Decimal(precision: 18, scale: 2),
                    F16 = c.Decimal(precision: 18, scale: 2),
                    F17 = c.Decimal(precision: 18, scale: 2),
                    Directemploymentbyage = c.Double(name: "Direct employment by age"),
                    F19 = c.Double(),
                    F20 = c.Double(),
                    F21 = c.Double(),
                    F22 = c.Double(),
                    F23 = c.Double(),
                    F24 = c.Double(),
                    Directemploymentbystatus = c.Double(name: "Direct employment by status"),
                    F26 = c.Double(),
                    F27 = c.Double(),
                    Netcapitalexpenditurem = c.Decimal(name: "Net capital expenditure (£m)", precision: 18, scale: 2),
                    F29 = c.Decimal(precision: 18, scale: 2),
                    Totaltaxestimatesm = c.Decimal(name: "Total tax estimates (£m)", precision: 18, scale: 2),
                    F31 = c.Decimal(precision: 18, scale: 2),
                    F32 = c.Decimal(precision: 18, scale: 2),
                    F33 = c.Decimal(precision: 18, scale: 2),
                    F34 = c.Decimal(precision: 18, scale: 2),
                    F35 = c.Decimal(precision: 18, scale: 2),
                    Directtaxestimatesm = c.Decimal(name: "Direct tax estimates (£m)", precision: 18, scale: 2),
                    F37 = c.Decimal(precision: 18, scale: 2),
                    F38 = c.Decimal(precision: 18, scale: 2),
                    F39 = c.Decimal(precision: 18, scale: 2),
                    F40 = c.Decimal(precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ContentPage", "CategoryID", "dbo.Category");
            DropForeignKey("Organisation.Tag", "OrgID", "Organisation.Org");
            DropForeignKey("Organisation.Tag", "FeatureID", "Organisation.Feature");
            DropForeignKey("Organisation.Feature", "FeatureTypeID", "Category.FeatureType");
            DropForeignKey("Organisation.Note", "OrgID", "Organisation.Org");
            DropForeignKey("Organisation.Org", "CountyID", "dbo.County");
            DropForeignKey("dbo.County", "RegionID", "dbo.Region");
            DropForeignKey("dbo.BookItem", "OrgID", "Organisation.Org");
            DropForeignKey("dbo.BookItem", "BookID", "dbo.Book");
            DropForeignKey("Organisation.Org", "AddressTypeID", "Category.AddressType");
            DropIndex("dbo.ContentPage", new[] {"CategoryID"});
            DropIndex("Organisation.Feature", new[] {"FeatureTypeID"});
            DropIndex("Organisation.Tag", new[] {"FeatureID"});
            DropIndex("Organisation.Tag", new[] {"OrgID"});
            DropIndex("Organisation.Note", new[] {"OrgID"});
            DropIndex("dbo.County", new[] {"RegionID"});
            DropIndex("dbo.BookItem", new[] {"OrgID"});
            DropIndex("dbo.BookItem", new[] {"BookID"});
            DropIndex("Organisation.Org", new[] {"CountyID"});
            DropIndex("Organisation.Org", new[] {"AddressTypeID"});
            DropTable("oe.LocalAuthorities");
            DropTable("dbo.ContentPage");
            DropTable("dbo.Category");
            DropTable("Category.FeatureType");
            DropTable("Organisation.Feature");
            DropTable("Organisation.Tag");
            DropTable("Organisation.Note");
            DropTable("dbo.Region");
            DropTable("dbo.County");
            DropTable("dbo.Book");
            DropTable("dbo.BookItem");
            DropTable("Organisation.Org");
            DropTable("Category.AddressType");
        }
    }
}