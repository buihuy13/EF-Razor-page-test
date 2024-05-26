using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EFRazor.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255,MinimumLength = 5, ErrorMessage = "Số kí tự phải thuộc khoảng {2} đến {1}")]
        [Required(ErrorMessage = "Trường dữ liệu bắt buộc")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Trường dữ liệu bắt buộc")]
        [DisplayName("Ngày tạo")]
        public DateTime Created { get; set; }
        public string Content { get; set; }

    }
}