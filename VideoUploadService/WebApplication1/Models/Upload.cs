namespace WebApplication1.Models
{
    public class Upload
    {
        public Guid Id { get; set; }
        public bool Done { get; set; } = false;
        public string? VideoLink { get; set; }
        public string? ThumbLink { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
