using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace porsOnlineApi.Migrations
{
    /// <inheritdoc />
    public partial class update00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SharedBy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SharedWith = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CanActive = table.Column<bool>(type: "bit", nullable: false),
                    IsStopped = table.Column<bool>(type: "bit", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    SubmittedResponses = table.Column<int>(type: "int", nullable: false),
                    PreviewCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReportCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UrlSlug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsTemplate = table.Column<bool>(type: "bit", nullable: false),
                    HasQuestion = table.Column<bool>(type: "bit", nullable: false),
                    Subdomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastResponseDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LabelsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TagsJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surveys_SurveyFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "SurveyFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppreciationPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HtmlTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ShareLinkActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    LinkActive = table.Column<bool>(type: "bit", nullable: false),
                    LinkButtonText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LinkType = table.Column<int>(type: "int", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReloadActive = table.Column<bool>(type: "bit", nullable: false),
                    ReloadTime = table.Column<int>(type: "int", nullable: false),
                    DescriptionText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppreciationPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppreciationPages_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HtmlTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ImageVideoActive = table.Column<bool>(type: "bit", nullable: false),
                    ImageOrVideo = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ShowCharts = table.Column<bool>(type: "bit", nullable: false),
                    DescriptionTextActive = table.Column<bool>(type: "bit", nullable: false),
                    DescriptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HtmlDescriptionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionNumberIsHidden = table.Column<bool>(type: "bit", nullable: false),
                    RespondingDuration = table.Column<int>(type: "int", nullable: true),
                    ImageBrightness = table.Column<int>(type: "int", nullable: false),
                    ImagePosition = table.Column<int>(type: "int", nullable: false),
                    DesktopImageLayout = table.Column<int>(type: "int", nullable: false),
                    MobileImageLayout = table.Column<int>(type: "int", nullable: false),
                    AnswerRequired = table.Column<bool>(type: "bit", nullable: false),
                    AllowMultipleSelect = table.Column<bool>(type: "bit", nullable: true),
                    MaxSelectableChoices = table.Column<int>(type: "int", nullable: true),
                    MinSelectableChoices = table.Column<int>(type: "int", nullable: true),
                    VerticalChoices = table.Column<bool>(type: "bit", nullable: true),
                    Randomize = table.Column<bool>(type: "bit", nullable: true),
                    Steps = table.Column<int>(type: "int", nullable: true),
                    IconType = table.Column<int>(type: "int", nullable: true),
                    AnswerType = table.Column<int>(type: "int", nullable: true),
                    NumberMaxValue = table.Column<long>(type: "bigint", nullable: true),
                    NumberMinValue = table.Column<long>(type: "bigint", nullable: true),
                    AnswerMaxLength = table.Column<int>(type: "int", nullable: true),
                    AnswerMinLength = table.Column<int>(type: "int", nullable: true),
                    IsDecimal = table.Column<bool>(type: "bit", nullable: true),
                    RegexType = table.Column<int>(type: "int", nullable: true),
                    RegexValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegexPlaceholder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegexValidationMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsThousandsSeparationEnabled = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    AuthenticationNeeded = table.Column<bool>(type: "bit", nullable: false),
                    PorslineAuth = table.Column<bool>(type: "bit", nullable: false),
                    CodeAuth = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumberAuth = table.Column<bool>(type: "bit", nullable: false),
                    EditResponseEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ShowAnswerSheetEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ShowAnswerSheetToResponder = table.Column<bool>(type: "bit", nullable: false),
                    ShowAnswerKeyAfterResponseSubmit = table.Column<bool>(type: "bit", nullable: false),
                    ShowAnswerKeyAfterSurveyStop = table.Column<bool>(type: "bit", nullable: false),
                    RespondingDurationType = table.Column<int>(type: "int", nullable: false),
                    QuestionsRespondingDuration = table.Column<int>(type: "int", nullable: true),
                    LocalStorageIsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    NoIndex = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveySettings_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuestionColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AnswerColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ButtonColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FontFamily = table.Column<int>(type: "int", nullable: false),
                    FontSize = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackgroundImageRepeat = table.Column<int>(type: "int", nullable: false),
                    BackgroundImageBrightness = table.Column<int>(type: "int", nullable: false),
                    BackgroundImageFit = table.Column<int>(type: "int", nullable: false),
                    BackgroundImagePosition = table.Column<int>(type: "int", nullable: false),
                    BackgroundImageSizePercentage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyThemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyThemes_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WelcomePages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HtmlTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TitleActive = table.Column<bool>(type: "bit", nullable: false),
                    DescriptionActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnterText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcomePages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WelcomePages_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Hidden = table.Column<bool>(type: "bit", nullable: false),
                    AltName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChoiceType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choices_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppreciationPages_SurveyId",
                table: "AppreciationPages",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_QuestionId",
                table: "Choices",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyId",
                table: "Questions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_Active",
                table: "Surveys",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_CreatedDate",
                table: "Surveys",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_FolderId",
                table: "Surveys",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_PreviewCode",
                table: "Surveys",
                column: "PreviewCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_ReportCode",
                table: "Surveys",
                column: "ReportCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveySettings_SurveyId",
                table: "SurveySettings",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyThemes_SurveyId",
                table: "SurveyThemes",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WelcomePages_SurveyId",
                table: "WelcomePages",
                column: "SurveyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppreciationPages");

            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "SurveySettings");

            migrationBuilder.DropTable(
                name: "SurveyThemes");

            migrationBuilder.DropTable(
                name: "WelcomePages");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "SurveyFolders");
        }
    }
}
