using System.ComponentModel.DataAnnotations;

namespace EFRazor.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Created { get; set; }
        public string Content { get; set; }

    }
}