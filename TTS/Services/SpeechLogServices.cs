using TTS.Data;
using TTS.Models;

namespace TTS.Services
{
    public class SpeechLogServices : ISpeechLogService
    {
        private readonly AppDbContext _context;
        public SpeechLogServices(AppDbContext context)
        {
            _context = context;
        }

        public List<SpeechLog> GetAllLogs()
        {
            return _context.SpeechLogs.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public  List<SpeechLog> GetLogsByUserId(int userId)
        {
            return _context.SpeechLogs.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToList();
        }

        public void addLogs(SpeechLog log)
        {
            _context.SpeechLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
