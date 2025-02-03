namespace NinicoFinalTask.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public bool isDeleted { get; set; }
    }
}
