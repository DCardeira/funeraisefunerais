using funeraisefunerais.Data;
using funeraisefunerais.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace funeraisefunerais.Pages;

public class ContactModel : PageModel
{
    private readonly AppDbContext _context;

    public ContactModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public ContactRequest Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.ContactRequests.Add(Input);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "O seu pedido foi enviado com sucesso.";
        return RedirectToPage();
    }
}
