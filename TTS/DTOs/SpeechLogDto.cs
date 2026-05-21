namespace TTS.DTOs
{
    public class SpeechLogDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string VoiceName { get; set; } = string.Empty;
        public double Rate { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
