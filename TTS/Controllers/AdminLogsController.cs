using Microsoft.AspNetCore.Mvc;
using TTS.Services;

namespace TTS.Controllers
{
    public class AdminLogsController : Controller
    {
        private readonly ISpeechLogService _speechLogService;
        private readonly ICurrentUserService _currentUserSevice;

        public AdminLogsController(ISpeechLogService speechLogService, ICurrentUserService currentUserSevice)
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
