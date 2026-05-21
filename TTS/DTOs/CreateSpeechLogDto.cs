using System.ComponentModel.DataAnnotations;

namespace TTS.DTOs
{
    public class CreateSpeechLogDto
    {
        [Required]
        public string Text { get; set; } = string.Empty;
        public string VoiceName { get; set; } = string.Empty;
        public double VoiceRate { get; set; } = 1;

    }
}
