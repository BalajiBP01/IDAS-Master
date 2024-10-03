using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inspirit.IDAS.Data.Migrations.ProductionDb
{
    public partial class InitProduction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditors",
                columns: table => new
                {
                    AuditorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuditorName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditors", x => x.AuditorID);
                });

            migrationBuilder.CreateTable(
                name: "Commercials",
                columns: table => new
                {
                    CommercialID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegistrationNo = table.Column<string>(nullable: true),
                    RegistrationNoOld = table.Column<string>(nullable: true),
                    CommercialName = table.Column<string>(nullable: true),
                    CommercialShortName = table.Column<string>(nullable: true),
                    CommercialTranslatedName = table.Column<string>(nullable: true),
                    PreviousBusinessname = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: true),
                    BusinessStartDate = table.Column<DateTime>(nullable: true),
                    FinancialYearEnd = table.Column<int>(nullable: true),
                    FinancialEffectiveDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    SICCode = table.Column<string>(nullable: true),
                    BusinessDesc = table.Column<string>(nullable: true),
                    CommercialStatusCode = table.Column<string>(nullable: true),
                    CommercialTypeCode = table.Column<string>(nullable: true),
                    VATNo = table.Column<long>(nullable: true),
                    BussEmail = table.Column<string>(nullable: true),
                    BussWebsite = table.Column<string>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: true),
                    IsESSynced = table.Column<bool>(nullable: false),
                    NoOfShares = table.Column<decimal>(type: "numeric(28, 9)", nullable: true),
                    AmountPerShare = table.Column<decimal>(type: "numeric(19, 1)", nullable: true),
                    Premium = table.Column<int>(nullable: true),
                    IDNo = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commercials", x => x.CommercialID);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerEmployments",
                columns: table => new
                {
                    ConsumerID = table.Column<long>(nullable: false),
                    ConsumerEmploymentID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployerDetail = table.Column<string>(nullable: true),
                    RecordStatusInd = table.Column<byte>(nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedOnDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerEmployments", x => x.ConsumerEmploymentID);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerOccupations",
                columns: table => new
                {
                    ConsumerEmploymentOccupationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConsumerID = table.Column<int>(nullable: true),
                    ConsumerEmploymentID = table.Column<int>(nullable: true),
                    Occupation = table.Column<string>(nullable: true),
                    RecordStatusInd = table.Column<byte>(nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerOccupations", x => x.ConsumerEmploymentOccupationID);
                });

            migrationBuilder.CreateTable(
                name: "Consumers",
                columns: table => new
                {
                    ConsumerID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IDNO = table.Column<string>(nullable: true),
                    PassportNo = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    SecondName = table.Column<string>(nullable: true),
                    ThirdName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    MaidenName = table.Column<string>(nullable: true),
                    GenderInd = table.Column<byte>(nullable: true),
                    RecordStatusInd = table.Column<byte>(nullable: false),
                    TitleCode = table.Column<string>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true),
                    FirstInitial = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false),
                    Alloy = table.Column<string>(nullable: true),
                    LSM = table.Column<string>(nullable: true),
                    IsESSynced = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumers", x => x.ConsumerID);
                });

            migrationBuilder.CreateTable(
                name: "Dashboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    InsertCount = table.Column<long>(nullable: false),
                    UpdateCount = table.Column<long>(nullable: false),
                    YearToDateUpdate = table.Column<long>(nullable: false),
                    TableName = table.Column<string>(nullable: true),
                    TotalCount = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    DirectorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstInitial = table.Column<string>(nullable: true),
                    SecondInitial = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    SecondName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    SurnameParticular = table.Column<string>(nullable: true),
                    SurnamePrevious = table.Column<string>(nullable: true),
                    IDNo = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DirectorStatusDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsRSAResidentYN = table.Column<bool>(nullable: true),
                    RegisterNo = table.Column<string>(nullable: true),
                    TrusteeOf = table.Column<string>(nullable: true),
                    MemberSize = table.Column<decimal>(nullable: true),
                    MemberControlPerc = table.Column<decimal>(nullable: true),
                    DirectorSetDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Profession = table.Column<string>(nullable: true),
                    Estate = table.Column<string>(nullable: true),
                    DirectorDesignationCode = table.Column<string>(nullable: true),
                    DirectorStatusCode = table.Column<string>(nullable: true),
                    DirectorTypeCode = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RecordStatusInd = table.Column<string>(maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.DirectorID);
                });

            migrationBuilder.CreateTable(
                name: "LastETLProcessedDate",
                columns: table => new
                {
                    ProcessedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastETLProcessedDate", x => x.ProcessedDate);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    LogId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LogTime = table.Column<DateTime>(nullable: false),
                    LogType = table.Column<string>(nullable: true),
                    TableName = table.Column<string>(nullable: true),
                    LogDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "LSM",
                columns: table => new
                {
                    CONSUMERID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IDNO = table.Column<string>(nullable: true),
                    RiskCategory = table.Column<string>(nullable: true),
                    IncomeCategory = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LSM", x => x.CONSUMERID);
                });

            migrationBuilder.CreateTable(
                name: "Postalcodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Suburb = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    Box_Code = table.Column<string>(nullable: true),
                    Str_Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postalcodes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDeeds",
                columns: table => new
                {
                    PropertyDeedID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeedsOfficeId = table.Column<int>(nullable: false),
                    TitleDeedNo = table.Column<string>(nullable: true),
                    TitleDeedNoOLD = table.Column<string>(nullable: true),
                    TitleDeedFee = table.Column<int>(nullable: true),
                    DatePurchase = table.Column<DateTime>(nullable: true),
                    DateRegister = table.Column<DateTime>(nullable: true),
                    PurchaseAmount = table.Column<decimal>(nullable: true),
                    StreetAddress = table.Column<string>(nullable: true),
                    StreetNumber = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    StreetType = table.Column<string>(nullable: true),
                    Y = table.Column<decimal>(nullable: true),
                    X = table.Column<decimal>(nullable: true),
                    SuburbCode = table.Column<string>(nullable: true),
                    SuburbDeeds = table.Column<string>(nullable: true),
                    Town = table.Column<string>(nullable: true),
                    Authority = table.Column<string>(nullable: true),
                    MunicipalityName = table.Column<string>(nullable: true),
                    ProvinceId = table.Column<byte>(nullable: true),
                    IsCurrentOwner = table.Column<bool>(nullable: true),
                    Extent = table.Column<string>(nullable: true),
                    AttorneyFirmNumber = table.Column<string>(nullable: true),
                    AttorneyFileNumber = table.Column<string>(nullable: true),
                    TransferSeqNo = table.Column<int>(nullable: true),
                    DateCaptured = table.Column<DateTime>(nullable: true),
                    BondNumber = table.Column<string>(nullable: true),
                    BondHolder = table.Column<string>(nullable: true),
                    BondAmount = table.Column<long>(nullable: true),
                    PropertyType = table.Column<string>(nullable: true),
                    PropertyName = table.Column<string>(nullable: true),
                    SchemeId = table.Column<string>(nullable: true),
                    SuburbId = table.Column<short>(nullable: true),
                    Erf = table.Column<string>(nullable: true),
                    Portion = table.Column<int>(nullable: true),
                    Unit = table.Column<int>(nullable: true),
                    CreatedOndate = table.Column<DateTime>(nullable: true),
                    ErfSize = table.Column<string>(nullable: true),
                    StandNo = table.Column<string>(nullable: true),
                    PortionNo = table.Column<string>(nullable: true),
                    TownShipNo = table.Column<int>(nullable: true),
                    PrevExtent = table.Column<string>(nullable: true),
                    IsCurrOwnerUpdated = table.Column<int>(nullable: true),
                    ChangedByLoaderID = table.Column<int>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDeeds", x => x.PropertyDeedID);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code_Start = table.Column<int>(nullable: false),
                    Code_End = table.Column<int>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TelephoneCodes",
                columns: table => new
                {
                    TelephoneCodeID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelephoneCodes", x => x.TelephoneCodeID);
                });

            migrationBuilder.CreateTable(
                name: "Telephones",
                columns: table => new
                {
                    TelephoneID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InternationalDialingCode = table.Column<string>(nullable: true),
                    TelephoneCode = table.Column<string>(nullable: true),
                    TelephoneNo = table.Column<long>(nullable: false),
                    RecordStatusInd = table.Column<byte>(nullable: false),
                    CreatedonDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telephones", x => x.TelephoneID);
                });

            migrationBuilder.CreateTable(
                name: "AuditorHistory",
                columns: table => new
                {
                    AuditorHistoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuditorID = table.Column<int>(nullable: false),
                    CommercialID = table.Column<long>(nullable: true),
                    CommercialAuditorID = table.Column<long>(nullable: true),
                    AuditorName = table.Column<string>(nullable: true),
                    ActStartDate = table.Column<DateTime>(nullable: true),
                    ActEndDate = table.Column<DateTime>(nullable: true),
                    LoadDate = table.Column<DateTime>(nullable: true),
                    AuditorStatusCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditorHistory", x => x.AuditorHistoryID);
                    table.ForeignKey(
                        name: "FK_AuditorHistory_Auditors_AuditorID",
                        column: x => x.AuditorID,
                        principalTable: "Auditors",
                        principalColumn: "AuditorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialAddresses",
                columns: table => new
                {
                    CommercialID = table.Column<int>(nullable: false),
                    CommercialAddressID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressTypeInd = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true),
                    OriginalAddress1 = table.Column<string>(nullable: true),
                    OriginalAddress2 = table.Column<string>(nullable: true),
                    OriginalAddress3 = table.Column<string>(nullable: true),
                    OriginalAddress4 = table.Column<string>(nullable: true),
                    OriginalPostalCode = table.Column<string>(nullable: true),
                    occupantTypeInd = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialAddresses", x => x.CommercialAddressID);
                    table.ForeignKey(
                        name: "FK_CommercialAddresses_Commercials_CommercialID",
                        column: x => x.CommercialID,
                        principalTable: "Commercials",
                        principalColumn: "CommercialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialAuditors",
                columns: table => new
                {
                    CommercialID = table.Column<int>(nullable: false),
                    CommercialAuditorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuditorName = table.Column<string>(nullable: true),
                    ActStartDate = table.Column<DateTime>(nullable: true),
                    ActEndDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    ProfessionCode = table.Column<string>(nullable: true),
                    AuditorTypeCode = table.Column<string>(nullable: true),
                    AuditorStatusCode = table.Column<string>(nullable: true),
                    ProfessionNo = table.Column<string>(nullable: true),
                    AuditorID = table.Column<int>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialAuditors", x => x.CommercialAuditorID);
                    table.ForeignKey(
                        name: "FK_CommercialAuditors_Commercials_CommercialID",
                        column: x => x.CommercialID,
                        principalTable: "Commercials",
                        principalColumn: "CommercialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialJudgements",
                columns: table => new
                {
                    CommercialID = table.Column<int>(nullable: false),
                    CommercialJudgmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommercialName = table.Column<string>(maxLength: 150, nullable: true),
                    Address1 = table.Column<string>(maxLength: 100, nullable: true),
                    Address2 = table.Column<string>(maxLength: 100, nullable: true),
                    Address3 = table.Column<string>(maxLength: 100, nullable: true),
                    Address4 = table.Column<string>(maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 10, nullable: true),
                    HomeTelephoneCode = table.Column<string>(maxLength: 5, nullable: true),
                    HomeTelephoneNo = table.Column<string>(maxLength: 50, nullable: true),
                    WorkTelephoneCode = table.Column<string>(maxLength: 5, nullable: true),
                    WorkTelephoneNo = table.Column<string>(maxLength: 50, nullable: true),
                    CellularNo = table.Column<string>(maxLength: 50, nullable: true),
                    FaxCode = table.Column<string>(maxLength: 5, nullable: true),
                    FaxNo = table.Column<string>(maxLength: 50, nullable: true),
                    CaseNumber = table.Column<string>(maxLength: 50, nullable: true),
                    CaseFilingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CaseType = table.Column<string>(maxLength: 100, nullable: true),
                    CaseReason = table.Column<string>(maxLength: 250, nullable: true),
                    DisputeAmt = table.Column<decimal>(type: "decimal(18, 9)", nullable: true),
                    CourtName = table.Column<string>(maxLength: 200, nullable: true),
                    CourtCity = table.Column<string>(maxLength: 200, nullable: true),
                    CourtType = table.Column<string>(maxLength: 200, nullable: true),
                    CourtCaseID = table.Column<int>(nullable: true),
                    PlaintiffName = table.Column<string>(maxLength: 200, nullable: true),
                    Plaintiff1Address = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyName = table.Column<string>(maxLength: 200, nullable: true),
                    AttorneyTelephoneCode = table.Column<string>(maxLength: 5, nullable: true),
                    AttorneyTelephoneNo = table.Column<string>(maxLength: 50, nullable: true),
                    AttorneyFaxCode = table.Column<string>(maxLength: 5, nullable: true),
                    AttorneyFaxNo = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyAddress1 = table.Column<string>(maxLength: 200, nullable: true),
                    AttorneyAddress2 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyAddress3 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyAddress4 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyPostalCode = table.Column<string>(maxLength: 100, nullable: true),
                    ReferenceNo = table.Column<string>(maxLength: 100, nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DisputeDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DisputeResolvedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Rescinded = table.Column<bool>(nullable: true),
                    RescissionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RescissionReason = table.Column<string>(maxLength: 200, nullable: true),
                    RescindedAmount = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialJudgements", x => x.CommercialJudgmentID);
                    table.ForeignKey(
                        name: "FK_CommercialJudgements_Commercials_CommercialID",
                        column: x => x.CommercialID,
                        principalTable: "Commercials",
                        principalColumn: "CommercialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialTelephones",
                columns: table => new
                {
                    CommercialID = table.Column<int>(nullable: false),
                    CommercialTelephoneID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TelephoneTypeInd = table.Column<string>(nullable: true),
                    TelephoneCode = table.Column<string>(nullable: true),
                    TelephoneNo = table.Column<string>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: true),
                    DeletedReason = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialTelephones", x => x.CommercialTelephoneID);
                    table.ForeignKey(
                        name: "FK_CommercialTelephones_Commercials_CommercialID",
                        column: x => x.CommercialID,
                        principalTable: "Commercials",
                        principalColumn: "CommercialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerAddresses",
                columns: table => new
                {
                    ConsumerAddressID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConsumerID = table.Column<long>(nullable: false),
                    AddressTypeInd = table.Column<int>(nullable: false),
                    OriginalAddress1 = table.Column<string>(nullable: true),
                    OriginalAddress2 = table.Column<string>(nullable: true),
                    OriginalAddress3 = table.Column<string>(nullable: true),
                    OriginalAddress4 = table.Column<string>(nullable: true),
                    OriginalPostalCode = table.Column<string>(nullable: true),
                    OccupantTypeInd = table.Column<string>(nullable: true),
                    RecordStatusInd = table.Column<int>(nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedOnDate = table.Column<DateTime>(nullable: true),
                    Town = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    ChangedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerAddresses", x => x.ConsumerAddressID);
                    table.ForeignKey(
                        name: "FK_ConsumerAddresses_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerDebtReviews",
                columns: table => new
                {
                    ConsumerDebtReviewID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConsumerID = table.Column<long>(nullable: false),
                    DebtCounsellorRegistrationNo = table.Column<string>(maxLength: 50, nullable: true),
                    DebtCounsellorFirstName = table.Column<string>(maxLength: 100, nullable: true),
                    DebtCounsellorSurname = table.Column<string>(maxLength: 100, nullable: true),
                    DebtCounsellorTelephoneCode = table.Column<string>(maxLength: 5, nullable: true),
                    DebtCounsellorTelephoneNo = table.Column<string>(maxLength: 50, nullable: true),
                    ApplicationCreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DebtReviewStatusDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RecordStatusInd = table.Column<byte>(nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DebtReviewStatusCode = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerDebtReviews", x => x.ConsumerDebtReviewID);
                    table.ForeignKey(
                        name: "FK_ConsumerDebtReviews_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerEmails",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmailID = table.Column<string>(maxLength: 200, nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ConsumerID = table.Column<long>(nullable: false),
                    RecordStatusInd = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerEmails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ConsumerEmails_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerEmploymentOccupations",
                columns: table => new
                {
                    ConsumerEmploymentOccupationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConsumerID = table.Column<long>(nullable: true),
                    ConsumerEmploymentID = table.Column<int>(nullable: true),
                    Occupation = table.Column<string>(nullable: true),
                    employer = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerEmploymentOccupations", x => x.ConsumerEmploymentOccupationID);
                    table.ForeignKey(
                        name: "FK_ConsumerEmploymentOccupations_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerHomeAffairs",
                columns: table => new
                {
                    HomeAffairsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IDNo = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    SecondName = table.Column<string>(nullable: true),
                    ThirdName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    NameCombo = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    HomeAffairsRunYN = table.Column<bool>(nullable: false),
                    HomeAffairsMessage = table.Column<string>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: true),
                    DeletedReason = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    HARecordChecksum = table.Column<int>(nullable: true),
                    SubscriberID = table.Column<int>(nullable: true),
                    LoaderID = table.Column<int>(nullable: true),
                    ConsumerID = table.Column<long>(nullable: false),
                    CreatedByUser = table.Column<string>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true),
                    ChangedByUser = table.Column<string>(nullable: true),
                    ChangedOnDate = table.Column<DateTime>(nullable: true),
                    IsPossibleDuplicateRecordYN = table.Column<bool>(nullable: true),
                    IsPossibleNameConflictYN = table.Column<bool>(nullable: true),
                    MaidenName = table.Column<string>(nullable: true),
                    IDIssuedDate = table.Column<DateTime>(nullable: true),
                    MarriageDate = table.Column<DateTime>(nullable: true),
                    PlaceOfMarriage = table.Column<string>(nullable: true),
                    SpouseIdnoOrDOB = table.Column<string>(nullable: true),
                    SpouseSurName = table.Column<string>(nullable: true),
                    SpouseForeNames = table.Column<string>(nullable: true),
                    DivorceDate = table.Column<DateTime>(nullable: true),
                    DivorceIssuedCourt = table.Column<string>(nullable: true),
                    DeceasedDate = table.Column<DateTime>(nullable: true),
                    DeceasedStatus = table.Column<string>(nullable: true),
                    PlaceOfDeath = table.Column<string>(nullable: true),
                    CauseOfDeath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerHomeAffairs", x => x.HomeAffairsID);
                    table.ForeignKey(
                        name: "FK_ConsumerHomeAffairs_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerJudgements",
                columns: table => new
                {
                    ConsumerJudgementID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConsumerID = table.Column<long>(nullable: false),
                    IDNo = table.Column<string>(maxLength: 13, nullable: true),
                    CaseNumber = table.Column<string>(maxLength: 50, nullable: false),
                    CaseFilingDate = table.Column<DateTime>(type: "date", nullable: true),
                    CaseReason = table.Column<string>(maxLength: 250, nullable: true),
                    CaseType = table.Column<string>(maxLength: 100, nullable: true),
                    DisputeAmt = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    CourtName = table.Column<string>(maxLength: 100, nullable: true),
                    CourtCity = table.Column<string>(maxLength: 100, nullable: true),
                    CourtType = table.Column<string>(maxLength: 100, nullable: true),
                    PlaintiffName = table.Column<string>(maxLength: 200, nullable: true),
                    PlaintiffAddress1 = table.Column<string>(maxLength: 100, nullable: true),
                    PlaintiffAddress2 = table.Column<string>(maxLength: 100, nullable: true),
                    PlaintiffAddress3 = table.Column<string>(maxLength: 100, nullable: true),
                    PlaintiffAddress4 = table.Column<string>(maxLength: 100, nullable: true),
                    PlaintiffPostalCode = table.Column<string>(maxLength: 10, nullable: true),
                    AttorneyName = table.Column<string>(maxLength: 200, nullable: true),
                    AttorneyTelephoneCode = table.Column<string>(maxLength: 5, nullable: true),
                    AttorneyTelephoneNo = table.Column<string>(maxLength: 50, nullable: true),
                    AttorneyFaxCode = table.Column<string>(maxLength: 5, nullable: true),
                    AttorneyFaxNo = table.Column<string>(maxLength: 50, nullable: true),
                    AttorneyAddress1 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyAddress2 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyAddress3 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyAddress4 = table.Column<string>(maxLength: 100, nullable: true),
                    AttorneyPostalCode = table.Column<string>(maxLength: 10, nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedOnDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    JudgementTypeCode = table.Column<string>(maxLength: 10, nullable: true),
                    DisputeDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DisputeResolvedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Rescinded = table.Column<bool>(nullable: true),
                    RescissionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RescissionReason = table.Column<string>(maxLength: 100, nullable: true),
                    RescindedAmount = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerJudgements", x => x.ConsumerJudgementID);
                    table.ForeignKey(
                        name: "FK_ConsumerJudgements_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialDirectors",
                columns: table => new
                {
                    CommercialID = table.Column<int>(nullable: false),
                    CommercialDirectorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DirectorID = table.Column<int>(nullable: true),
                    AppointmentDate = table.Column<DateTime>(nullable: true),
                    DirectorStatusDate = table.Column<DateTime>(nullable: true),
                    IsRSAResidentYN = table.Column<bool>(nullable: true),
                    RegisterNo = table.Column<string>(nullable: true),
                    TrusteeOf = table.Column<string>(nullable: true),
                    MemberSize = table.Column<decimal>(nullable: true),
                    MemberControlPerc = table.Column<decimal>(nullable: true),
                    DirectorSetDate = table.Column<DateTime>(nullable: true),
                    Profession = table.Column<string>(nullable: true),
                    Estate = table.Column<string>(nullable: true),
                    DirectorDesignationCode = table.Column<string>(nullable: true),
                    DirectorStatusCode = table.Column<string>(nullable: true),
                    DirectorTypeCode = table.Column<string>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialDirectors", x => x.CommercialDirectorID);
                    table.ForeignKey(
                        name: "FK_CommercialDirectors_Commercials_CommercialID",
                        column: x => x.CommercialID,
                        principalTable: "Commercials",
                        principalColumn: "CommercialID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommercialDirectors_Directors_DirectorID",
                        column: x => x.DirectorID,
                        principalTable: "Directors",
                        principalColumn: "DirectorID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DirectorAddresses",
                columns: table => new
                {
                    DirectorID = table.Column<int>(nullable: false),
                    DirectorAddressID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    addressTypeInd = table.Column<string>(nullable: true),
                    province = table.Column<string>(nullable: true),
                    originalAddress1 = table.Column<string>(nullable: true),
                    originalAddress2 = table.Column<string>(nullable: true),
                    originalAddress3 = table.Column<string>(nullable: true),
                    originalAddress4 = table.Column<string>(nullable: true),
                    originalPostalCode = table.Column<string>(nullable: true),
                    occupantTypeInd = table.Column<string>(nullable: true),
                    lastUpdatedDate = table.Column<DateTime>(nullable: false),
                    createdOnDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectorAddresses", x => x.DirectorAddressID);
                    table.ForeignKey(
                        name: "FK_DirectorAddresses_Directors_DirectorID",
                        column: x => x.DirectorID,
                        principalTable: "Directors",
                        principalColumn: "DirectorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectorTelephones",
                columns: table => new
                {
                    DirectorID = table.Column<int>(nullable: false),
                    DirectorTelephoneID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TelephoneTypeInd = table.Column<string>(nullable: true),
                    TelephoneCode = table.Column<string>(nullable: true),
                    TelephoneNo = table.Column<string>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: true),
                    DeletedReason = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectorTelephones", x => x.DirectorTelephoneID);
                    table.ForeignKey(
                        name: "FK_DirectorTelephones_Directors_DirectorID",
                        column: x => x.DirectorID,
                        principalTable: "Directors",
                        principalColumn: "DirectorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Endorsements",
                columns: table => new
                {
                    EndorsementID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PropertyDeedID = table.Column<int>(nullable: true),
                    EndorsementNumber = table.Column<string>(nullable: true),
                    EndorsementHolder = table.Column<string>(nullable: true),
                    EndorsementAmount = table.Column<int>(nullable: true),
                    RecordStatusInd = table.Column<string>(nullable: true),
                    CreateByUser = table.Column<string>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: false),
                    ChangedByUser = table.Column<string>(nullable: true),
                    ChangedOnDate = table.Column<DateTime>(nullable: true),
                    DeedsLoaderID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endorsements", x => x.EndorsementID);
                    table.ForeignKey(
                        name: "FK_Endorsements_PropertyDeeds_PropertyDeedID",
                        column: x => x.PropertyDeedID,
                        principalTable: "PropertyDeeds",
                        principalColumn: "PropertyDeedID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDeedBuyers",
                columns: table => new
                {
                    BuyerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PropertyDeedId = table.Column<int>(nullable: true),
                    BuyerIDNO = table.Column<string>(nullable: true),
                    BuyerName = table.Column<string>(nullable: true),
                    BuyerType = table.Column<byte>(nullable: true),
                    BuyerStatus = table.Column<string>(nullable: true),
                    Share = table.Column<string>(nullable: true),
                    CreatedOndate = table.Column<DateTime>(nullable: false),
                    RecordStatusInd = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDeedBuyers", x => x.BuyerID);
                    table.ForeignKey(
                        name: "FK_PropertyDeedBuyers_PropertyDeeds_PropertyDeedId",
                        column: x => x.PropertyDeedId,
                        principalTable: "PropertyDeeds",
                        principalColumn: "PropertyDeedID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDeedSellers",
                columns: table => new
                {
                    SellerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PropertyDeedId = table.Column<int>(nullable: true),
                    SellerIDNO = table.Column<string>(nullable: true),
                    SellerName = table.Column<string>(nullable: true),
                    SellerType = table.Column<byte>(nullable: true),
                    SellerStatus = table.Column<string>(nullable: true),
                    CreatedOndate = table.Column<DateTime>(nullable: false),
                    RecordStatusInd = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDeedSellers", x => x.SellerID);
                    table.ForeignKey(
                        name: "FK_PropertyDeedSellers_PropertyDeeds_PropertyDeedId",
                        column: x => x.PropertyDeedId,
                        principalTable: "PropertyDeeds",
                        principalColumn: "PropertyDeedID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsumerTelephones",
                columns: table => new
                {
                    ConsumerTelephoneID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConsumerID = table.Column<long>(nullable: false),
                    TelephoneID = table.Column<long>(nullable: false),
                    TelephoneTypeInd = table.Column<byte>(nullable: true),
                    RecordStatusInd = table.Column<byte>(nullable: false),
                    CreatedonDate = table.Column<DateTime>(nullable: false),
                    ChangedonDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerTelephones", x => x.ConsumerTelephoneID);
                    table.ForeignKey(
                        name: "FK_ConsumerTelephones_Consumers_ConsumerID",
                        column: x => x.ConsumerID,
                        principalTable: "Consumers",
                        principalColumn: "ConsumerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumerTelephones_Telephones_TelephoneID",
                        column: x => x.TelephoneID,
                        principalTable: "Telephones",
                        principalColumn: "TelephoneID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditorAddresses",
                columns: table => new
                {
                    AuditorID = table.Column<int>(nullable: true),
                    AuditorAddressID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressTypeInd = table.Column<string>(nullable: true),
                    OriginalAddress1 = table.Column<string>(nullable: true),
                    OriginalAddress2 = table.Column<string>(nullable: true),
                    OriginalAddress3 = table.Column<string>(nullable: true),
                    OriginalAddress4 = table.Column<string>(nullable: true),
                    OriginalPostalCode = table.Column<string>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditorAddresses", x => x.AuditorAddressID);
                    table.ForeignKey(
                        name: "FK_AuditorAddresses_CommercialAuditors_AuditorID",
                        column: x => x.AuditorID,
                        principalTable: "CommercialAuditors",
                        principalColumn: "CommercialAuditorID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    ConsumerAddressID = table.Column<long>(nullable: true),
                    CommercialAddressID = table.Column<long>(nullable: true),
                    CommercialAddressID1 = table.Column<int>(nullable: true),
                    DirectorAddressID = table.Column<long>(nullable: true),
                    DirectorAddressID1 = table.Column<int>(nullable: true),
                    AddressDetial = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Suburb = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Addresses_CommercialAddresses_CommercialAddressID1",
                        column: x => x.CommercialAddressID1,
                        principalTable: "CommercialAddresses",
                        principalColumn: "CommercialAddressID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_ConsumerAddresses_ConsumerAddressID",
                        column: x => x.ConsumerAddressID,
                        principalTable: "ConsumerAddresses",
                        principalColumn: "ConsumerAddressID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_DirectorAddresses_DirectorAddressID1",
                        column: x => x.DirectorAddressID1,
                        principalTable: "DirectorAddresses",
                        principalColumn: "DirectorAddressID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CommercialAddressID1",
                table: "Addresses",
                column: "CommercialAddressID1");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ConsumerAddressID",
                table: "Addresses",
                column: "ConsumerAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DirectorAddressID1",
                table: "Addresses",
                column: "DirectorAddressID1");

            migrationBuilder.CreateIndex(
                name: "IX_AuditorAddresses_AuditorID",
                table: "AuditorAddresses",
                column: "AuditorID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditorHistory_AuditorID",
                table: "AuditorHistory",
                column: "AuditorID");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialAddresses_CommercialID",
                table: "CommercialAddresses",
                column: "CommercialID");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialAuditors_CommercialID",
                table: "CommercialAuditors",
                column: "CommercialID");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialDirectors_CommercialID",
                table: "CommercialDirectors",
                column: "CommercialID");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialDirectors_DirectorID",
                table: "CommercialDirectors",
                column: "DirectorID");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialJudgements_CommercialID",
                table: "CommercialJudgements",
                column: "CommercialID");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialTelephones_CommercialID",
                table: "CommercialTelephones",
                column: "CommercialID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerAddresses_ConsumerID",
                table: "ConsumerAddresses",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerDebtReviews_ConsumerID",
                table: "ConsumerDebtReviews",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerEmails_ConsumerID",
                table: "ConsumerEmails",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerEmploymentOccupations_ConsumerID",
                table: "ConsumerEmploymentOccupations",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerHomeAffairs_ConsumerID",
                table: "ConsumerHomeAffairs",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerJudgements_ConsumerID",
                table: "ConsumerJudgements",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_Consumers_IDNO",
                table: "Consumers",
                column: "IDNO");

            migrationBuilder.CreateIndex(
                name: "IX_Consumers_PassportNo",
                table: "Consumers",
                column: "PassportNo");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerTelephones_ConsumerID",
                table: "ConsumerTelephones",
                column: "ConsumerID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumerTelephones_TelephoneID",
                table: "ConsumerTelephones",
                column: "TelephoneID");

            migrationBuilder.CreateIndex(
                name: "IX_DirectorAddresses_DirectorID",
                table: "DirectorAddresses",
                column: "DirectorID");

            migrationBuilder.CreateIndex(
                name: "IX_DirectorTelephones_DirectorID",
                table: "DirectorTelephones",
                column: "DirectorID");

            migrationBuilder.CreateIndex(
                name: "IX_Endorsements_PropertyDeedID",
                table: "Endorsements",
                column: "PropertyDeedID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDeedBuyers_PropertyDeedId",
                table: "PropertyDeedBuyers",
                column: "PropertyDeedId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDeedSellers_PropertyDeedId",
                table: "PropertyDeedSellers",
                column: "PropertyDeedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AuditorAddresses");

            migrationBuilder.DropTable(
                name: "AuditorHistory");

            migrationBuilder.DropTable(
                name: "CommercialDirectors");

            migrationBuilder.DropTable(
                name: "CommercialJudgements");

            migrationBuilder.DropTable(
                name: "CommercialTelephones");

            migrationBuilder.DropTable(
                name: "ConsumerDebtReviews");

            migrationBuilder.DropTable(
                name: "ConsumerEmails");

            migrationBuilder.DropTable(
                name: "ConsumerEmploymentOccupations");

            migrationBuilder.DropTable(
                name: "ConsumerEmployments");

            migrationBuilder.DropTable(
                name: "ConsumerHomeAffairs");

            migrationBuilder.DropTable(
                name: "ConsumerJudgements");

            migrationBuilder.DropTable(
                name: "ConsumerOccupations");

            migrationBuilder.DropTable(
                name: "ConsumerTelephones");

            migrationBuilder.DropTable(
                name: "Dashboards");

            migrationBuilder.DropTable(
                name: "DirectorTelephones");

            migrationBuilder.DropTable(
                name: "Endorsements");

            migrationBuilder.DropTable(
                name: "LastETLProcessedDate");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "LSM");

            migrationBuilder.DropTable(
                name: "Postalcodes");

            migrationBuilder.DropTable(
                name: "PropertyDeedBuyers");

            migrationBuilder.DropTable(
                name: "PropertyDeedSellers");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "TelephoneCodes");

            migrationBuilder.DropTable(
                name: "CommercialAddresses");

            migrationBuilder.DropTable(
                name: "ConsumerAddresses");

            migrationBuilder.DropTable(
                name: "DirectorAddresses");

            migrationBuilder.DropTable(
                name: "CommercialAuditors");

            migrationBuilder.DropTable(
                name: "Auditors");

            migrationBuilder.DropTable(
                name: "Telephones");

            migrationBuilder.DropTable(
                name: "PropertyDeeds");

            migrationBuilder.DropTable(
                name: "Consumers");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "Commercials");
        }
    }
}
