using funeraisefunerais.Data;
using funeraisefunerais.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace funeraisefunerais.Pages;

public class ContactRequestsModel : PageModel
{
    private readonly AppDbContext _context;

    public ContactRequestsModel(AppDbContext context)
    {
        _context = context;
    }

    public List<ContactRequest> Pedidos { get; set; } = new();

    public async Task OnGetAsync()
    {
        Pedidos = await _context.ContactRequests
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();
    }
}
