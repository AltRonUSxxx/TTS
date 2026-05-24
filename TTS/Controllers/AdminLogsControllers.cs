using Microsoft.AspNetCore.Mvc;
using TTS.Services;

namespace TTS.Controllers
{
    public class AdminLogsControllers : Controller
    {
        private readonly ISpeechLogService _speechLogService;
        private readonly ICurrentUserSevice _currentUserSevice;

        public AdminLogsControllers(ISpeechLogService speechLogService, ICurrentUserSevice currentUserSevice)
        {
            _speechLogService = speechLogService;
            _currentUserSevice = currentUserSevice;
        }

        public IActionResult Index()
        {
            if(!_currentUserSevice.IsLoggedIn(HttpContext))
            {
                return RedirectToPage("/index");
            }
            var log = _speechLogService.GetAllLogs();
            return View(log);
        }
    }
}
