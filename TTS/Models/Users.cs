namespace TTS.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
