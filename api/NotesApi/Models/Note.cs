namespace NotesApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        public int UserId { get; set; } // owner of the note
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}