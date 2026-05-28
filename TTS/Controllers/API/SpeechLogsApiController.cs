using Microsoft.AspNetCore.Mvc;
using TTS.DTOs;
using TTS.Models;
using TTS.Services;

namespace TTS.Controllers.API
{
    [ApiController]
    [Route("/api/speechlogs")]
    public class SpeechLogsApiController : ControllerBase
    {
        private readonly ISpeechLogService _speechLogService;
        private readonly ICurrentUserService _currentUserSevice;

        public SpeechLogsApiController(ISpeechLogService speechLogService, ICurrentUserService currentUserSevice)
        {
            _speechLogService = speechLogService;
            _currentUserSevice = currentUserSevice;
        }

        [HttpGet]
        public ActionResult<List<SpeechLogDto>> GetAll()
        {
            var logs = _speechLogService.GetAllLogs();
            var res = logs.Select(log => ToDto(log));
            return Ok(res);
        }

        private static SpeechLogDto ToDto(SpeechLog log)
        {
            return new SpeechLogDto
            {
                Id = log.Id,
                Text = log.Text,
                VoiceName = log.VoiceName,
                Rate = log.Rate,
                CreatedAt = log.CreatedAt,
                UserName = log.User?.Name
            };
        }

        [HttpPost]
        public IActionResult Create(CreateSpeechLogDto dto)
        {
            var userId = _currentUserSevice.GetCurrentUserId(HttpContext);
            if(userId == null)
            {
                return Unauthorized(new {message = "Need to login in account"});
            }
            var log = new SpeechLog
            {
                Text = dto.Text,
                VoiceName = dto.VoiceName,
                Rate = dto.VoiceRate,
                CreatedAt = DateTime.Now,
                UserId = userId.Value
            };
            _speechLogService.addLogs(log);
            return Ok(new { message = "log saved" });

        }
    }
}
