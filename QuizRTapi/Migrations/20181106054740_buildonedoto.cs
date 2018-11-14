using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizRTapi.Migrations
{
    public partial class buildonedoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionsT",
                columns: table => new
                {
                    QuestionsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Categ = table.Column<string>(nullable: true),
                    Topic = table.Column<string>(nullable: true),
                    QuestionGiven = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionsT", x => x.QuestionsId);
                });

            migrationBuilder.CreateTable(
                name: "QuizRTTemplateT",
                columns: table => new
                {
                    TempId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    SparQL = table.Column<string>(nullable: true),
                    Categ = table.Column<string>(nullable: true),
                    CategName = table.Column<string>(nullable: true),
                    Topic = table.Column<string>(nullable: true),
                    TopicName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizRTTemplateT", x => x.TempId);
                });

            migrationBuilder.CreateTable(
                name: "OptionsT",
                columns: table => new
                {
                    OptionsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OptionGiven = table.Column<string>(nullable: true),
                    IsCorrect = table.Column<bool>(nullable: false),
                    QuestionsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionsT", x => x.OptionsId);
                    table.ForeignKey(
                        name: "FK_OptionsT_QuestionsT_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "QuestionsT",
                        principalColumn: "QuestionsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionsT_QuestionsId",
                table: "OptionsT",
                column: "QuestionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionsT");

            migrationBuilder.DropTable(
                name: "QuizRTTemplateT");

            migrationBuilder.DropTable(
                name: "QuestionsT");
        }
    }
}
