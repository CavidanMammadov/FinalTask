using System.ComponentModel.DataAnnotations;

namespace NinicoFinalTask.ViewModel.Slider
{
    public class SliderUpdateVM
    {

        [MaxLength(32, ErrorMessage = "Title must be less than 32 character"), Required(ErrorMessage = "You must enter title")]
        public string Title { get; set; }
        [MaxLength(32, ErrorMessage = "SubTitle must be less than 32 character"), Required(ErrorMessage = "You must enter SubTitle")]
        public string SubTitle { get; set; }
        public string? Link { get; set; }
        [Required(ErrorMessage = "You must enter start price")]
        public int StartPrice { get; set; }
        [Required(ErrorMessage = "You must enter file")]
        public IFormFile File { get; set; }
    }
}
