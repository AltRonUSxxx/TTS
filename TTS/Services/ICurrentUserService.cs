using TTS.Models;
namespace TTS.Services
{
    public interface ICurrentUserService
    {
        bool IsLoggedIn(HttpContext httpContext);
        int? GetCurrentUserId(HttpContext httpContext);
        User? GetCurrentUser(HttpContext httpContext);
        void SignIn(HttpContext httpContext, int UserId);
        void SignOut(HttpContext httpContext);
    }
}
