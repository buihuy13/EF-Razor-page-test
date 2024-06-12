using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFRazor.Migrations
{
    /// <inheritdoc />
    public partial class seedusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i=0;i<150;i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAddress"
                    },
                    values: new object[] {
                        Guid.NewGuid().ToString(), //Phat sinh ra id ngau nhien
                        "User-" + i.ToString(),
                        $"email{i}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0,
                        "...@#%..."
                    }
                );

            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
