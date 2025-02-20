using NinicoFinalTask.ViewModel.Common;
using System.ComponentModel.DataAnnotations;

namespace NinicoFinalTask.ViewModel.Blog
{
    public class BlogUpdateVM
    {
        [MaxLength(32, ErrorMessage = "Title of blog must be less than 32"), Required(ErrorMessage = "You must enter title for create Blog")]
        public string Title { get; set; }
        [MaxLength(128, ErrorMessage = "Subtitle of blog must be less than 32"), Required(ErrorMessage = "You must enter Subtitle for create Blog")]
        public string SubTitle { get; set; }
        [MaxLength(1024, ErrorMessage = "Description must be less than 1024 character"), Required(ErrorMessage = "You must enter description for blog ")]
        public string Description { get; set; }
        public string CoverFileUrl { get; set; }
        public IEnumerable<ImageUrlAndId> OtherImagesUrl { get; set; }
        public IFormFile CoverFile { get; set; } = null!;
        public IEnumerable<IFormFile>? OtherImages { get; set; }
    }
}
