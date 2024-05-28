using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using EFRazor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EFRazor.Models
{
    public class BlogContext : IdentityDbContext<appUser>
    {
        //Khai báo giống như vậy để sau này đăng ký nó như 1 hệ thống DI của dịch vụ
        //Khi BlogContext được tạo ra, nó sẽ inject vào
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
            //...
        }
        public DbSet<Article> articles { get; set; }

        public string InitData()
        {
            var stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder["Port"] = "3306";
            stringBuilder["UID"] = "root";
            stringBuilder["Database"] = "Blog";
            stringBuilder["PWD"] = "huydaica";
            stringBuilder["Server"] = "localhost";

            return stringBuilder.ToString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL(InitData());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Xóa đi AspNet có trong tên của các table csdl Identity
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}