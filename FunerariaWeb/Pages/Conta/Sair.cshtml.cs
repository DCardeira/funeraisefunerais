using FunerariaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunerariaWeb.Pages.Conta
{
    public class SairModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SairModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
