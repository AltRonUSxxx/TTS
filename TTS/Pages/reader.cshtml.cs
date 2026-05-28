using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;
using TTS.Data;
using TTS.Models;
using TTS.Services;

namespace TTS.Pages
{
    public class readerModel : PageModel
    {
        private readonly ICurrentUserService _currentUserSevice;
        public readerModel(ICurrentUserService currentUserSevice)
        {
            _currentUserSevice = currentUserSevice;
        }

        public IActionResult OnGet()
        {
            if(!_currentUserSevice.IsLoggedIn(HttpContext))
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}
