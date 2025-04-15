using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotionConnect.Migrations
{
    /// <inheritdoc />
    public partial class FixGillningarConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ForNamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EfterNamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilBildUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FodelsAr = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArProfilOppen = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chattar",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArGruppChat = table.Column<bool>(type: "bit", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkapadTid = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chattar", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "Grupper",
                columns: table => new
                {
                    GruppId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GruppNamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkapadesTid = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupper", x => x.GruppId);
                });

            migrationBuilder.CreateTable(
                name: "Sporter",
                columns: table => new
                {
                    SportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Namn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sporter", x => x.SportId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inlagg",
                columns: table => new
                {
                    InlaggId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BildUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkapadesTid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnvandarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnvandareId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inlagg", x => x.InlaggId);
                    table.ForeignKey(
                        name: "FK_Inlagg_AspNetUsers_AnvandareId",
                        column: x => x.AnvandareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notiser",
                columns: table => new
                {
                    NotisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Meddealande = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArLast = table.Column<bool>(type: "bit", nullable: false),
                    SkapadesTid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnvandarId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notiser", x => x.NotisId);
                    table.ForeignKey(
                        name: "FK_Notiser_AspNetUsers_AnvandarId",
                        column: x => x.AnvandarId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vanner",
                columns: table => new
                {
                    AnvandarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VanId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vanner", x => new { x.AnvandarId, x.VanId });
                    table.ForeignKey(
                        name: "FK_Vanner_AspNetUsers_AnvandarId",
                        column: x => x.AnvandarId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vanner_AspNetUsers_VanId",
                        column: x => x.VanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meddelanden",
                columns: table => new
                {
                    MeddelandeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkapadesTid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvsandareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meddelanden", x => x.MeddelandeId);
                    table.ForeignKey(
                        name: "FK_Meddelanden_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Meddelanden_AspNetUsers_AvsandareId",
                        column: x => x.AvsandareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Meddelanden_Chattar_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chattar",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GruppMedlemmar",
                columns: table => new
                {
                    GruppId = table.Column<int>(type: "int", nullable: false),
                    AnvandarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GruppMedlemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruppMedlemmar", x => new { x.GruppId, x.AnvandarId });
                    table.ForeignKey(
                        name: "FK_GruppMedlemmar_AspNetUsers_AnvandarId",
                        column: x => x.AnvandarId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GruppMedlemmar_Grupper_GruppId",
                        column: x => x.GruppId,
                        principalTable: "Grupper",
                        principalColumn: "GruppId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnvandareSporter",
                columns: table => new
                {
                    AnvandarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnvandareSporter", x => new { x.AnvandarId, x.SportId });
                    table.ForeignKey(
                        name: "FK_AnvandareSporter_AspNetUsers_AnvandarId",
                        column: x => x.AnvandarId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnvandareSporter_Sporter_SportId",
                        column: x => x.SportId,
                        principalTable: "Sporter",
                        principalColumn: "SportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InlaggSporter",
                columns: table => new
                {
                    InlaggId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InlaggSporter", x => new { x.InlaggId, x.SportId });
                    table.ForeignKey(
                        name: "FK_InlaggSporter_Inlagg_InlaggId",
                        column: x => x.InlaggId,
                        principalTable: "Inlagg",
                        principalColumn: "InlaggId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InlaggSporter_Sporter_SportId",
                        column: x => x.SportId,
                        principalTable: "Sporter",
                        principalColumn: "SportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kommentarer",
                columns: table => new
                {
                    KommentarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkapadTid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InlaggId = table.Column<int>(type: "int", nullable: false),
                    AnvandarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnvandareId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kommentarer", x => x.KommentarId);
                    table.ForeignKey(
                        name: "FK_Kommentarer_AspNetUsers_AnvandareId",
                        column: x => x.AnvandareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Kommentarer_Inlagg_InlaggId",
                        column: x => x.InlaggId,
                        principalTable: "Inlagg",
                        principalColumn: "InlaggId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeddelandeMottagare",
                columns: table => new
                {
                    MeddelandeMottagreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeddelandeId = table.Column<int>(type: "int", nullable: false),
                    MottagareId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeddelandeMottagare", x => x.MeddelandeMottagreId);
                    table.ForeignKey(
                        name: "FK_MeddelandeMottagare_AspNetUsers_MottagareId",
                        column: x => x.MottagareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeddelandeMottagare_Meddelanden_MeddelandeId",
                        column: x => x.MeddelandeId,
                        principalTable: "Meddelanden",
                        principalColumn: "MeddelandeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gillningar",
                columns: table => new
                {
                    GillingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InlaggId = table.Column<int>(type: "int", nullable: true),
                    KommentarId = table.Column<int>(type: "int", nullable: true),
                    AnvandarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnvandareId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gillningar", x => x.GillingsId);
                    table.ForeignKey(
                        name: "FK_Gillningar_AspNetUsers_AnvandareId",
                        column: x => x.AnvandareId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Gillningar_Inlagg_InlaggId",
                        column: x => x.InlaggId,
                        principalTable: "Inlagg",
                        principalColumn: "InlaggId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gillningar_Kommentarer_KommentarId",
                        column: x => x.KommentarId,
                        principalTable: "Kommentarer",
                        principalColumn: "KommentarId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnvandareSporter_SportId",
                table: "AnvandareSporter",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Gillningar_AnvandareId",
                table: "Gillningar",
                column: "AnvandareId");

            migrationBuilder.CreateIndex(
                name: "IX_Gillningar_InlaggId",
                table: "Gillningar",
                column: "InlaggId");

            migrationBuilder.CreateIndex(
                name: "IX_Gillningar_KommentarId",
                table: "Gillningar",
                column: "KommentarId");

            migrationBuilder.CreateIndex(
                name: "IX_GruppMedlemmar_AnvandarId",
                table: "GruppMedlemmar",
                column: "AnvandarId");

            migrationBuilder.CreateIndex(
                name: "IX_Inlagg_AnvandareId",
                table: "Inlagg",
                column: "AnvandareId");

            migrationBuilder.CreateIndex(
                name: "IX_InlaggSporter_SportId",
                table: "InlaggSporter",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Kommentarer_AnvandareId",
                table: "Kommentarer",
                column: "AnvandareId");

            migrationBuilder.CreateIndex(
                name: "IX_Kommentarer_InlaggId",
                table: "Kommentarer",
                column: "InlaggId");

            migrationBuilder.CreateIndex(
                name: "IX_MeddelandeMottagare_MeddelandeId",
                table: "MeddelandeMottagare",
                column: "MeddelandeId");

            migrationBuilder.CreateIndex(
                name: "IX_MeddelandeMottagare_MottagareId",
                table: "MeddelandeMottagare",
                column: "MottagareId");

            migrationBuilder.CreateIndex(
                name: "IX_Meddelanden_ApplicationUserId",
                table: "Meddelanden",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meddelanden_AvsandareId",
                table: "Meddelanden",
                column: "AvsandareId");

            migrationBuilder.CreateIndex(
                name: "IX_Meddelanden_ChatId",
                table: "Meddelanden",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Notiser_AnvandarId",
                table: "Notiser",
                column: "AnvandarId");

            migrationBuilder.CreateIndex(
                name: "IX_Vanner_VanId",
                table: "Vanner",
                column: "VanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnvandareSporter");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Gillningar");

            migrationBuilder.DropTable(
                name: "GruppMedlemmar");

            migrationBuilder.DropTable(
                name: "InlaggSporter");

            migrationBuilder.DropTable(
                name: "MeddelandeMottagare");

            migrationBuilder.DropTable(
                name: "Notiser");

            migrationBuilder.DropTable(
                name: "Vanner");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Kommentarer");

            migrationBuilder.DropTable(
                name: "Grupper");

            migrationBuilder.DropTable(
                name: "Sporter");

            migrationBuilder.DropTable(
                name: "Meddelanden");

            migrationBuilder.DropTable(
                name: "Inlagg");

            migrationBuilder.DropTable(
                name: "Chattar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
