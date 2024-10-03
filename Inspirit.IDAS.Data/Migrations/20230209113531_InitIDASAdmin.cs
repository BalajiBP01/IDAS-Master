using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inspirit.IDAS.Data.Migrations
{
    public partial class InitIDASAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Message = table.Column<string>(maxLength: 250, nullable: true),
                    Showmessage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SettingName = table.Column<string>(nullable: true),
                    SettingValue = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatchProcessFileGeneration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BatchId = table.Column<Guid>(nullable: false),
                    CustomerUserId = table.Column<Guid>(nullable: false),
                    IdNumber = table.Column<string>(nullable: true),
                    Executed = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ExecutedDate = table.Column<DateTime>(nullable: true),
                    CustomerID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchProcessFileGeneration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 100, nullable: true),
                    Business = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Message = table.Column<string>(maxLength: 500, nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TradingName = table.Column<string>(maxLength: 100, nullable: false),
                    RegistrationName = table.Column<string>(maxLength: 100, nullable: false),
                    RegistrationNumber = table.Column<string>(nullable: false),
                    VATNumber = table.Column<string>(nullable: true),
                    BranchLocation = table.Column<string>(maxLength: 100, nullable: true),
                    PhysicalAddress = table.Column<string>(maxLength: 250, nullable: false),
                    TypeOfBusiness = table.Column<string>(nullable: false),
                    TelephoneNumber = table.Column<string>(nullable: false),
                    FaxNumber = table.Column<string>(nullable: true),
                    BillingEmail = table.Column<string>(maxLength: 100, nullable: true),
                    Status = table.Column<string>(nullable: true),
                    BillingType = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Turnover = table.Column<bool>(nullable: false),
                    CustOwnIDNumber = table.Column<string>(nullable: true),
                    PostalAddress = table.Column<string>(nullable: true),
                    WebAddress = table.Column<string>(nullable: true),
                    AccountDeptContactPerson = table.Column<string>(nullable: true),
                    AccountDeptTelephoneNumber = table.Column<string>(nullable: true),
                    AccountDeptFaxNumber = table.Column<string>(nullable: true),
                    AuthIDNumber = table.Column<string>(nullable: true),
                    AuthPosition = table.Column<string>(nullable: true),
                    AccountDeptEmail = table.Column<string>(maxLength: 100, nullable: true),
                    AuthFirstName = table.Column<string>(nullable: true),
                    AuthSurName = table.Column<string>(nullable: true),
                    AuthCellNumber = table.Column<string>(nullable: false),
                    AuthEmail = table.Column<string>(nullable: true),
                    BusinessDescription = table.Column<string>(nullable: true),
                    CreditBureauInformation = table.Column<string>(nullable: true),
                    Purpose = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<Guid>(nullable: true),
                    ActivatedBy = table.Column<Guid>(nullable: true),
                    ActivatedDate = table.Column<DateTime>(nullable: true),
                    IsRestricted = table.Column<bool>(nullable: true),
                    Client_Logo = table.Column<string>(nullable: true),
                    TabSelected = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataServicesAgreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataServicesAgreements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoNotCallRegistrys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Emailid = table.Column<string>(nullable: true),
                    Idnumber = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoNotCallRegistrys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    MailContent = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    Attachment = table.Column<string>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadFileGeneration",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    LeadId = table.Column<Guid>(nullable: false),
                    CustomerUserId = table.Column<Guid>(nullable: false),
                    LeadOutput = table.Column<string>(nullable: true),
                    Executed = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ExecutedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    CustomerID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadFileGeneration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LeadsGenaration",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CustomerUserID = table.Column<Guid>(nullable: false),
                    LeadsDate = table.Column<DateTime>(nullable: false),
                    RequestedRecors = table.Column<int>(nullable: false),
                    AdminCertified = table.Column<bool>(nullable: true),
                    ProFormaInvoiceId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    ApprovedOnDate = table.Column<DateTime>(nullable: true),
                    ApprovedBy = table.Column<Guid>(nullable: true),
                    LeadsNumber = table.Column<int>(nullable: false),
                    OutPutFileName = table.Column<string>(nullable: true),
                    InputDetail = table.Column<string>(nullable: true),
                    OutputDetail = table.Column<string>(nullable: true),
                    MaritalStaus = table.Column<string>(nullable: true),
                    RiskCategories = table.Column<string>(nullable: true),
                    AlloyBreakDowns = table.Column<string>(nullable: true),
                    LocationServices = table.Column<string>(nullable: true),
                    AgeGroupGenders = table.Column<string>(nullable: true),
                    ProfileGender = table.Column<string>(nullable: true),
                    IncomeBrackets = table.Column<string>(nullable: true),
                    TotalRecordsAvailable = table.Column<string>(nullable: true),
                    ProfileReport = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadsGenaration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LookupDatas",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupDatas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    BlogName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategorys",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategorys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductDataTypes",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDataTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrailUsers",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: true),
                    BusinessRegisterNumber = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    isExpired = table.Column<bool>(nullable: false),
                    Credits = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrailUsers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LoginName = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Emailid = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProFormaInvoices",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ProFormaInvoiceNumber = table.Column<int>(nullable: false),
                    IsProformal = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ReferenceNumber = table.Column<int>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    VatTotal = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    IsSubmitted = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    IsEmailSend = table.Column<bool>(nullable: false),
                    EmailSendDate = table.Column<DateTime>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: true),
                    ProformaDisplyNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProFormaInvoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProFormaInvoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    SubDisplayNumber = table.Column<string>(nullable: true),
                    IsAutoBilled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerDSAs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataServicesAgreementId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDSAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerDSAs_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerDSAs_DataServicesAgreements_DataServicesAgreementId",
                        column: x => x.DataServicesAgreementId,
                        principalTable: "DataServicesAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ActivatedDate = table.Column<DateTime>(nullable: false),
                    DeactivatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false),
                    ServiceId = table.Column<Guid>(nullable: false),
                    UsageType = table.Column<string>(nullable: true),
                    IsPostpaid = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    BatchProduct = table.Column<bool>(nullable: true),
                    LeadProduct = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrailUserLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    TrailUserId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ProductPackageId = table.Column<Guid>(nullable: false),
                    CreditPoints = table.Column<int>(nullable: false),
                    Idorpassportnumber = table.Column<string>(nullable: true),
                    Searchtype = table.Column<string>(nullable: true),
                    SearchCriteria = table.Column<string>(nullable: true),
                    Logtype = table.Column<string>(nullable: true),
                    InputType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrailUserLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TrailUserLogs_TrailUsers_TrailUserId",
                        column: x => x.TrailUserId,
                        principalTable: "TrailUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    FormName = table.Column<string>(nullable: true),
                    ViewAction = table.Column<bool>(nullable: false),
                    AddAction = table.Column<bool>(nullable: false),
                    EditAction = table.Column<bool>(nullable: false),
                    privileged = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Title = table.Column<string>(maxLength: 10, nullable: false),
                    IDNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Password = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    SubscriptionId = table.Column<Guid>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<Guid>(nullable: true),
                    ActivatedBy = table.Column<Guid>(nullable: true),
                    ActivatedDate = table.Column<DateTime>(nullable: true),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    BatchwithoutSub = table.Column<bool>(nullable: true),
                    IsUserLoggedIn = table.Column<bool>(nullable: true),
                    IsRestricted = table.Column<bool>(nullable: true),
                    LeadswithoutSub = table.Column<bool>(nullable: true),
                    MAchAddressCHK = table.Column<bool>(nullable: true),
                    MacAddresses = table.Column<string>(nullable: true),
                    LastPasswordResetDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerUsers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerUsers_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    InvoiceNumber = table.Column<int>(nullable: false),
                    ProFormaInvoice = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ReferenceNumber = table.Column<int>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    VatTotal = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    BillingType = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    SubscriptionID = table.Column<Guid>(nullable: true),
                    ispaid = table.Column<bool>(nullable: false),
                    IsCreditNoteRaised = table.Column<bool>(nullable: false),
                    isSubmited = table.Column<bool>(nullable: false),
                    isCancelled = table.Column<bool>(nullable: false),
                    IsEmailSend = table.Column<bool>(nullable: false),
                    EmailSendDate = table.Column<DateTime>(nullable: false),
                    InvoiceDate = table.Column<DateTime>(nullable: false),
                    InvoiceDisplayNumber = table.Column<string>(nullable: true),
                    IsTaxinvSent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Subscriptions_SubscriptionID",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: true),
                    ModifiedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerProducts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPackageRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    MinLimit = table.Column<int>(nullable: false),
                    MaxLimit = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPackageRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPackageRates_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceLineItems",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ProformaInvoiceId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceLineItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceLineItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceLineItems_ProFormaInvoices_ProformaInvoiceId",
                        column: x => x.ProformaInvoiceId,
                        principalTable: "ProFormaInvoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchTraces",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CustomerUserID = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    OutPutFileName = table.Column<string>(nullable: true),
                    BatchNumber = table.Column<int>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    TotalRecords = table.Column<int>(nullable: false),
                    FoundRecords = table.Column<int>(nullable: false),
                    AgeGroupGenders = table.Column<string>(nullable: true),
                    ProfileGender = table.Column<string>(nullable: true),
                    IncomeBrackets = table.Column<string>(nullable: true),
                    MaritalStaus = table.Column<string>(nullable: true),
                    RiskCategories = table.Column<string>(nullable: true),
                    AlloyBreakDowns = table.Column<string>(nullable: true),
                    LocationServices = table.Column<string>(nullable: true),
                    TotalRecordsAvailable = table.Column<string>(nullable: true),
                    IsDataDownloaded = table.Column<bool>(nullable: false),
                    ProFormaInvoiceId = table.Column<Guid>(nullable: true),
                    AdminCertified = table.Column<bool>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    ApprovedOnDate = table.Column<DateTime>(nullable: false),
                    ApprovedBy = table.Column<Guid>(nullable: true),
                    IdNumbers = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchTraces", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BatchTraces_CustomerUsers_CustomerUserID",
                        column: x => x.CustomerUserID,
                        principalTable: "CustomerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Creditnotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    CreditNoteNumber = table.Column<int>(nullable: false),
                    CreditNoteValue = table.Column<decimal>(nullable: false),
                    CreditNoteDate = table.Column<DateTime>(nullable: false),
                    Comments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditnotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creditnotes_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    PaymentType = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    PaymentAmountReceive = table.Column<decimal>(nullable: true),
                    PaymentReceivedDate = table.Column<DateTime>(nullable: true),
                    Comments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLineItems",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    InvoiceID = table.Column<Guid>(nullable: false),
                    ProductPackageRateID = table.Column<Guid>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    NetAmount = table.Column<decimal>(nullable: false),
                    VatAmount = table.Column<decimal>(nullable: false),
                    BillingType = table.Column<string>(maxLength: 20, nullable: true),
                    UsageType = table.Column<string>(maxLength: 20, nullable: true),
                    SubscriptionID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLineItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_ProductPackageRates_ProductPackageRateID",
                        column: x => x.ProductPackageRateID,
                        principalTable: "ProductPackageRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_Subscriptions_SubscriptionID",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductPackageId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    BillingType = table.Column<string>(nullable: true),
                    Duration = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    SubscriptionId = table.Column<Guid>(nullable: false),
                    isBilled = table.Column<bool>(nullable: true),
                    ProRataPrice = table.Column<decimal>(nullable: false),
                    ProRataNetAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionItems_ProductPackageRates_ProductPackageId",
                        column: x => x.ProductPackageId,
                        principalTable: "ProductPackageRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionItems_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionLicences",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    SubscriptionItemId = table.Column<Guid>(nullable: true),
                    CustomerUserId = table.Column<Guid>(nullable: false),
                    AssignedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionLicences", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubscriptionLicences_CustomerUsers_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "CustomerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionLicences_SubscriptionItems_SubscriptionItemId",
                        column: x => x.SubscriptionItemId,
                        principalTable: "SubscriptionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workorders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    SubscriptionItemID = table.Column<Guid>(nullable: true),
                    ProductPackageId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    ServiceType = table.Column<string>(nullable: true),
                    Credits = table.Column<int>(nullable: false),
                    isCancelled = table.Column<bool>(nullable: false),
                    InvoiceLineItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workorders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workorders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workorders_ProductPackageRates_ProductPackageId",
                        column: x => x.ProductPackageId,
                        principalTable: "ProductPackageRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workorders_SubscriptionItems_SubscriptionItemID",
                        column: x => x.SubscriptionItemID,
                        principalTable: "SubscriptionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    CustomerUserId = table.Column<Guid>(nullable: true),
                    CompanyUserId = table.Column<Guid>(nullable: false),
                    CreditPoints = table.Column<int>(nullable: false),
                    IdOrPassportNumber = table.Column<string>(nullable: true),
                    SearchType = table.Column<string>(nullable: true),
                    SearchCriteria = table.Column<string>(nullable: true),
                    LogType = table.Column<string>(nullable: true),
                    WorkorderId = table.Column<Guid>(nullable: true),
                    InputType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerLogs_CustomerUsers_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "CustomerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerLogs_Workorders_WorkorderId",
                        column: x => x.WorkorderId,
                        principalTable: "Workorders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchTraces_CustomerUserID",
                table: "BatchTraces",
                column: "CustomerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Creditnotes_InvoiceId",
                table: "Creditnotes",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDSAs_CustomerId",
                table: "CustomerDSAs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDSAs_DataServicesAgreementId",
                table: "CustomerDSAs",
                column: "DataServicesAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLogs_CustomerUserId",
                table: "CustomerLogs",
                column: "CustomerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLogs_WorkorderId",
                table: "CustomerLogs",
                column: "WorkorderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProducts_CustomerId",
                table: "CustomerProducts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProducts_ProductId",
                table: "CustomerProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_CustomerId",
                table: "CustomerUsers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_SubscriptionId",
                table: "CustomerUsers",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_InvoiceID",
                table: "InvoiceLineItems",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_ProductPackageRateID",
                table: "InvoiceLineItems",
                column: "ProductPackageRateID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_SubscriptionID",
                table: "InvoiceLineItems",
                column: "SubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SubscriptionID",
                table: "Invoices",
                column: "SubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPackageRates_ProductId",
                table: "ProductPackageRates",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ServiceId",
                table: "Products",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceLineItems_ProductId",
                table: "ProformaInvoiceLineItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceLineItems_ProformaInvoiceId",
                table: "ProformaInvoiceLineItems",
                column: "ProformaInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProFormaInvoices_CustomerId",
                table: "ProFormaInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionItems_ProductPackageId",
                table: "SubscriptionItems",
                column: "ProductPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionItems_SubscriptionId",
                table: "SubscriptionItems",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionLicences_CustomerUserId",
                table: "SubscriptionLicences",
                column: "CustomerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionLicences_SubscriptionItemId",
                table: "SubscriptionLicences",
                column: "SubscriptionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CustomerId",
                table: "Subscriptions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrailUserLogs_TrailUserId",
                table: "TrailUserLogs",
                column: "TrailUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workorders_CustomerId",
                table: "Workorders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Workorders_ProductPackageId",
                table: "Workorders",
                column: "ProductPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Workorders_SubscriptionItemID",
                table: "Workorders",
                column: "SubscriptionItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationMessages");

            migrationBuilder.DropTable(
                name: "ApplicationSetting");

            migrationBuilder.DropTable(
                name: "BatchProcessFileGeneration");

            migrationBuilder.DropTable(
                name: "BatchTraces");

            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "Creditnotes");

            migrationBuilder.DropTable(
                name: "CustomerDSAs");

            migrationBuilder.DropTable(
                name: "CustomerLogs");

            migrationBuilder.DropTable(
                name: "CustomerProducts");

            migrationBuilder.DropTable(
                name: "DoNotCallRegistrys");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "InvoiceAttachments");

            migrationBuilder.DropTable(
                name: "InvoiceLineItems");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "LeadFileGeneration");

            migrationBuilder.DropTable(
                name: "LeadsGenaration");

            migrationBuilder.DropTable(
                name: "LookupDatas");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductCategorys");

            migrationBuilder.DropTable(
                name: "ProductDataTypes");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceLineItems");

            migrationBuilder.DropTable(
                name: "SubscriptionLicences");

            migrationBuilder.DropTable(
                name: "TrailUserLogs");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "DataServicesAgreements");

            migrationBuilder.DropTable(
                name: "Workorders");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "ProFormaInvoices");

            migrationBuilder.DropTable(
                name: "CustomerUsers");

            migrationBuilder.DropTable(
                name: "TrailUsers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SubscriptionItems");

            migrationBuilder.DropTable(
                name: "ProductPackageRates");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
