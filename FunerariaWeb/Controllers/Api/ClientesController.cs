using FunerariaWeb.Data;
using FunerariaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FunerariaWeb.Controllers.Api
{
    // Componente 2 do trabalho: API REST em ASP.NET Core MVC (Controllers),
    // separada das Razor Pages, mas partilhando o mesmo DbContext/EF Core.
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/clientes
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetTodos()
        {
            var clientes = await _context.Clientes
                .Select(c => new { c.Id, c.Nome, c.Email, c.Telefone, TotalCerimonias = c.Cerimonias.Count })
                .ToListAsync();
            return Ok(clientes);
        }

        // GET api/clientes/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Cliente>> GetPorId(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente is null) return NotFound(new { mensagem = "Cliente não encontrado." });
            return Ok(cliente);
        }

        // POST api/clientes
        [HttpPost]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<ActionResult<Cliente>> Criar(Cliente cliente)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _context.Clientes.AnyAsync(c => c.Email == cliente.Email))
                return Conflict(new { mensagem = "Já existe um cliente com este email." });

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPorId), new { id = cliente.Id }, cliente);
        }

        // PUT api/clientes/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> Atualizar(int id, Cliente cliente)
        {
            if (id != cliente.Id) return BadRequest(new { mensagem = "Id do URL não corresponde ao Id do corpo." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(cliente).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clientes.AnyAsync(c => c.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        // DELETE api/clientes/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> Apagar(int id)
        {
            var cliente = await _context.Clientes.Include(c => c.Cerimonias).FirstOrDefaultAsync(c => c.Id == id);
            if (cliente is null) return NotFound();
            if (cliente.Cerimonias.Any())
                return Conflict(new { mensagem = "Cliente com cerimónias associadas não pode ser apagado." });

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
