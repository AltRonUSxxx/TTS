using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TTS.Data;
using TTS.Models;
using TTS.Services;

namespace TTS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passHasher;
        private readonly ICurrentUserSevice _currentUserSevice;

        public IndexModel(AppDbContext context, ICurrentUserSevice currentUserSevice)
        {
            _passHasher = new PasswordHasher<User>();
            _context = context;
            _currentUserSevice = currentUserSevice;
        }

        [BindProperty]
        public string RegisterName { get; set; } = string.Empty;
        [BindProperty]
        public string RegisterLogin { get; set; } = string.Empty;
        [BindProperty]
        public string RegisterPassword { get; set; } = string.Empty;
        [BindProperty]
        public string RegisterConfrimPassword { get; set; } = string.Empty;

        [BindProperty]
        public string LoginLogin { get; set; } = string.Empty;
        [BindProperty]
        public string LoginPassword { get; set; } = string.Empty;

        public bool IsAuthorized { get; set; }
        public int CurrentUserId { get; set; }
        public string Message { get; set; }

        private void LoadCurrentUser()
        {
            var user = _currentUserSevice.GetCurrentUser(HttpContext);
            if(user == null)
            {
                IsAuthorized = false;
                return;
            }
            IsAuthorized = true;
            CurrentUserId = user.Id;
        }

        public IActionResult OnPostRegiser()
        {
            LoadCurrentUser();

            if(RegisterPassword != RegisterConfrimPassword)
            {
                Message = "Passwords should be same";
                return Page();
            }
            if(string.IsNullOrWhiteSpace(RegisterLogin) ||
                string.IsNullOrEmpty(RegisterPassword) ||
                string.IsNullOrEmpty(RegisterName))
            {
                Message = "Fill all fields";
                return Page();
            }

            bool loginExist = _context.Users.Any(u => u.Login == RegisterLogin);

            if(loginExist)
            {
                Message = "This login alreay taken";
                return Page();
            }

            var user = new User
            {
                Name = RegisterName,
                Login = RegisterLogin
            };
            user.HashedPassword = _passHasher.HashPassword(user, RegisterPassword);
            _context.Users.Add(user);
            _context.SaveChanges();
            _currentUserSevice.SignIn(HttpContext, user.Id);

            return RedirectToPage("/Reader");
        }

        public IActionResult OnPostLogin()
        {
            LoadCurrentUser();

            if (string.IsNullOrWhiteSpace(LoginLogin) ||
                string.IsNullOrEmpty(LoginPassword))
            {
                Message = "Fill all fields";
                return Page();
            }

            User user = _context.Users.FirstOrDefault(u => u.Login == RegisterLogin);

            if (user.Login != LoginLogin)
            {
                Message = "Uncorret login or password";
                return Page();
            }

            var result = _passHasher.VerifyHashedPassword(user, user.HashedPassword, LoginPassword);
            if(result == PasswordVerificationResult.Failed)
            {
                Message = "Uncorret login or password";
                return Page();
            }

            _currentUserSevice.SignIn(HttpContext, user.Id);

            return RedirectToPage("/Reader");
        }



        public IActionResult OnPostLogOut()
        {
            _currentUserSevice.SignOut(HttpContext);
            return RedirectToPage();
        }



        public void OnGet()
        {

        }
    }
}
