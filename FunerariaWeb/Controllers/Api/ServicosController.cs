using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Servico>>> GetTodos()
        {
            return Ok(await _context.Servicos.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Servico>> GetPorId(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico is null) return NotFound();
            return Ok(servico);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<ActionResult<Servico>> Criar(Servico servico)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPorId), new { id = servico.Id }, servico);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> Atualizar(int id, Servico servico)
        {
            if (id != servico.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(servico).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> Apagar(int id)
        {
            var servico = await _context.Servicos.Include(s => s.CerimoniaServicos).FirstOrDefaultAsync(s => s.Id == id);
            if (servico is null) return NotFound();
            if (servico.CerimoniaServicos.Any())
                return Conflict(new { mensagem = "Serviço em uso não pode ser apagado." });

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
