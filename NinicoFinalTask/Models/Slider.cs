using System.ComponentModel.DataAnnotations;

namespace NinicoFinalTask.Models
{
    public class Slider :BaseEntity
    {
        [MaxLength(32,ErrorMessage ="Title must be less than 32 character"), ]
        public string Title { get; set; } = null!;
        [MaxLength(48, ErrorMessage = "Title must be less than 48  character")]
        public string SubTitle { get; set; } = null!;
        public string? Link { get; set; }
        public int StartPrice { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
