using TTS.Models;

namespace TTS.Services
{
    public interface ISpeechLogService
    {
        List<SpeechLog> GetAllLogs();
        List<SpeechLog> GetLogsByUserId(int userId);
        void addLogs(SpeechLog log);
    }
}
