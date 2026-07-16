using FunerariaWeb.Data;
using FunerariaWeb.Hubs;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CerimoniasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificadorCerimonias _notificador;

        public CerimoniasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, NotificadorCerimonias notificador)
        {
            _context = context;
            _userManager = userManager;
            _notificador = notificador;
        }

        // GET api/cerimonias  (Admin: todas; Cliente: só as suas)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTodas()
        {
            var query = _context.Cerimonias
                .Include(c => c.Cliente)
                .Include(c => c.CerimoniaServicos).ThenInclude(cs => cs.Servico)
                .AsQueryable();

            if (!User.IsInRole(Roles.Administrador))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(c => c.Cliente != null && c.Cliente.IdentityUserId == userId);
            }

            var resultado = await query.Select(c => new
            {
                c.Id,
                c.DataCerimonia,
                c.Local,
                Estado = c.Estado.ToString(),
                Cliente = c.Cliente!.Nome,
                Servicos = c.CerimoniaServicos.Select(cs => new { cs.Servico!.Nome, cs.Quantidade, cs.PrecoUnitario }),
                c.Total
            }).ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetPorId(int id)
        {
            var cerimonia = await _context.Cerimonias
                .Include(c => c.Cliente)
                .Include(c => c.CerimoniaServicos).ThenInclude(cs => cs.Servico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cerimonia is null) return NotFound();

            if (!User.IsInRole(Roles.Administrador))
            {
                var userId = _userManager.GetUserId(User);
                if (cerimonia.Cliente?.IdentityUserId != userId) return Forbid();
            }

            return Ok(cerimonia);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<ActionResult> Criar(Cerimonia cerimonia)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Cerimonias.Add(cerimonia);
            await _context.SaveChangesAsync();
            await _notificador.AvisarAlteracaoAsync($"Nova cerimónia criada via API (Id {cerimonia.Id}).");

            return CreatedAtAction(nameof(GetPorId), new { id = cerimonia.Id }, cerimonia);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> Apagar(int id)
        {
            var cerimonia = await _context.Cerimonias.Include(c => c.CerimoniaServicos).FirstOrDefaultAsync(c => c.Id == id);
            if (cerimonia is null) return NotFound();

            _context.CerimoniaServicos.RemoveRange(cerimonia.CerimoniaServicos);
            _context.Cerimonias.Remove(cerimonia);
            await _context.SaveChangesAsync();
            await _notificador.AvisarAlteracaoAsync($"Cerimónia Id {id} apagada via API.");

            return NoContent();
        }
    }
}
