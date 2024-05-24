using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Cms;
using EFRazor.Models;
#nullable disable

namespace EFRazor.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            //Tạo random
            Randomizer.Seed = new Random(8675309);
            //Tạo ra đối tượng Faker vào Article để có thể Seed Database
            var fakerArticle = new Faker<Article>();
            //Set quy luật cho từng thuộc tính của Article
            //Đối với Title => sẽ có đội dài tối thiểu là 5 và tối đa là 10 từ
            fakerArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(5, 5));
            //Created sẽ nằm trong khoảng 15/1/2005 đến 13/4/2005
            fakerArticle.RuleFor(a => a.Created, f => f.Date.Between(new DateTime(2005, 1, 15), new DateTime(2005, 4, 13)));
            //Content sẽ có số đoạn văn nằm trong khoảng từ 1 đến 4
            fakerArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 4));

            //Insert data
            
            for (int i=0;i<100;i++)
            {
               Article article = fakerArticle.Generate();
               migrationBuilder.InsertData(
               table: "articles",
               columns: new[] { "Title", "Created", "Content" },
               values: new object[]
               {
                   article.Title,
                   article.Created,
                   article.Content
               }
               );
            }    
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
